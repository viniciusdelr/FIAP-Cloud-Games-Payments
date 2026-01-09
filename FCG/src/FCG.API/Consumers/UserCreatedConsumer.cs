using FCG.API.Events;
using FCG.Domain.Entities;
using FCG.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FCG.API.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(IServiceProvider serviceProvider, ILogger<UserCreatedConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation($"[MassTransit] Recebido usuário: {message.UserId}");

            // O MassTransit gerencia o escopo, mas para garantir acesso ao banco Scoped:
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                // Lógica de criar a Wallet (igual fizemos antes)
                var existingWallet = await dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == message.UserId);
                if (existingWallet == null)
                {
                    dbContext.Wallets.Add(new Wallet { Id = message.UserId, Funds = 0, Username = message.Username });
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
