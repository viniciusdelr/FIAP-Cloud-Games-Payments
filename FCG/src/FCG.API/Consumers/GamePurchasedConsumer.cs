using FCG.API.Events;
using FCG.API.Shared.Events;
using FCG.Domain.Entities;
using FCG.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Payments.API.Consumers
{
    public class GamePurchasedConsumer : IConsumer<GamePurchased>
    {
        private readonly DataContext _context;
        public GamePurchasedConsumer(DataContext context) { _context = context; }

        //public async Task Consume(ConsumeContext<GamePurchased> consumeContext)
        //{
        //    var msg = consumeContext.Message;

        //    var wallet = await _context.Wallets
        //                                   .FirstOrDefaultAsync(w => w.UserId == );

        //    // --- LÓGICA DE NEGÓCIO ---
        //    // Exemplo: aprovamos apenas se o valor for menor que 100 reais
        //    bool temSaldo = msg.Price <= 100.00m;
        //    string justificativa = temSaldo ? "Saldo aprovado!" : "Saldo insuficiente na carteira.";

        //    // --- RESPOSTA (O "Pulo do Gato") ---
        //    // Publicamos o resultado de volta para que o Games possa ouvir
        //    await consumeContext.Publish(new PaymentResult
        //    {
        //        CorrelationId = msg.CorrelationId,
        //        Success = temSaldo,
        //        Message = justificativa
        //    });

        //    Console.WriteLine($"[PAYMENTS] Processada transação {msg.CorrelationId}. Resultado: {temSaldo}");
        //}

        public async Task Consume(ConsumeContext<GamePurchased> consumeContext)
        {
            var msg = consumeContext.Message;

            // 1. Busca a carteira do usuário no banco de dados
            var wallet = await _context.Wallets
                                       .FirstOrDefaultAsync(w => w.UserId == msg.UserId);

            bool temSaldo = false;
            string justificativa;

            if (wallet == null)
            {
                justificativa = "Carteira não encontrada para este usuário.";
            }
            else if (wallet.Funds >= msg.Price) // 2. Valida se o saldo é suficiente
            {
                temSaldo = true;
                justificativa = "Saldo aprovado!";

                // 3. (Opcional) Subtrai o valor imediatamente ou aguarda confirmação final
                wallet.Funds -= msg.Price;
                _context.Wallets.Update(wallet);
                await _context.SaveChangesAsync();
            }
            else
            {
                justificativa = $"Saldo insuficiente. Você tem {wallet.Funds:C2} e o jogo custa {msg.Price:C2}.";
            }

            // --- RESPOSTA PARA O SERVIÇO DE GAMES ---
            await consumeContext.Publish(new PaymentResult
            {
                CorrelationId = msg.CorrelationId,
                Success = temSaldo,
                Message = justificativa
            });

            Console.WriteLine($"[PAYMENTS] Usuário {msg.UserId} - Transação {msg.CorrelationId}. Status: {temSaldo}");
        }

    }
}
