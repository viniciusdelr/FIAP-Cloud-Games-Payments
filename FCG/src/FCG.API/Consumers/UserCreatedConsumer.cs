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

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                var existingWallet = await dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == message.UserId);

                if (existingWallet == null)
                {
                    var newWallet = new Wallet
                    {
                        UserId = message.UserId, 
                        Username = message.Username ?? "User",
                        Funds = 0,
                        LastRecharge = DateTime.UtcNow
                    };

                    dbContext.Wallets.Add(newWallet);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Carteira criada para User {message.UserId}");
                }
            }
        }
    }
}
