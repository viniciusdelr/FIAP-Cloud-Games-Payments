# FIAP Cloud Games - Payments API

Microsserviço focado na gestão financeira (Wallet) e processamento de transações. Ele atua majoritariamente em "background", reagindo a eventos de compra.

## Arquitetura e Lógica

Este serviço é o validador financeiro do ecossistema:
* **Wallet**: Gerencia o saldo dos usuários.
* **Processamento de Compra (Consumer)**: O serviço escuta o evento `GamePurchased` vindo do Azure Service Bus.
  * Verifica se o usuário tem saldo suficiente.
  * Se **SIM**: Debita o valor e emite evento de Sucesso.
  * Se **NÃO**: Emite evento de Falha.
* **Idempotência**: Garante que a mesma mensagem de compra não gere débitos duplicados.

## Tecnologias

* .NET 8
* Entity Framework Core
* SQL Server / Azure SQL
* MassTransit
* Azure Service Bus
* Docker

## Configuração (Variáveis de Ambiente)

Configurações obrigatórias para conexão com filas e banco:

* `ConnectionStrings:DefaultConnection`: Conexão com o banco de dados de Pagamentos/Wallet.
* `ConnectionStrings:ServiceBusConnection`: Endpoint do Azure Service Bus.
* `Jwt:Key`: Chave para validação de token (usada nos endpoints de recarga e consulta).

## Execução Local (Docker)

1. Construir a imagem:
   docker build -t fcg-payments .

2. Rodar o container:
   docker run -d -p 8082:80 \
     -e "ConnectionStrings:DefaultConnection=SUA_SQL_CONN_STRING" \
     -e "ConnectionStrings:ServiceBusConnection=SUA_SB_CONN_STRING" \
     --name fcg-payments \
     fcg-payments

## Deploy na Azure (Kubernetes)

A estratégia de deploy no AKS considera a natureza de processamento em background:
1. O serviço mantém uma conexão persistente com o Azure Service Bus para receber mensagens.
2. Em caso de falha no processamento, a mensagem é movida para uma **Dead Letter Queue (DLQ)** para análise posterior, garantindo que nenhum pedido financeiro seja perdido.
3. Utiliza **Liveness Probes** do Kubernetes para reiniciar o pod caso a conexão com o Service Bus seja perdida.

## Endpoints

### Wallet
* `POST /Recharge`: Adiciona créditos à conta do usuário.
  * **Input**: Valor, UserId.
  * **Segurança**: Requer Token JWT.
* `GET /GetWallet/{userId}`: Consulta o saldo atual.

### Payments
* `GET /SearchUserPurchases`: Histórico de transações processadas (Aprovadas ou Recusadas).

## Testes

Os testes unitários focam principalmente na lógica de débito da Wallet e concorrência.

dotnet test