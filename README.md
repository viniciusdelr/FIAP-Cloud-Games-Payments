# FIAP-Cloud-Games-Payments
```markdown
# FCG Payments Microservice

Microsserviço responsável pela gestão financeira dos usuários (Wallet) e validação de transações.

## Funcionalidades

* **Wallet**: Consulta de saldo e recarga de créditos.
* **Histórico**: Consulta de compras realizadas e status de transações.
* **Processamento**: Validação de saldo para compras de jogos.

## Endpoints Principais

* `POST /Recharge`: Adiciona créditos à carteira do usuário (Ex: R$ 100,00).
* `GET /GetWallet/{userId}`: Retorna saldo atual e dados da carteira.
* `GET /SearchUserPurchases`: Histórico de transações.

## Lógica de Mensageria (MassTransit)

Este serviço atua primariamente como **Consumer**:

1. Escuta o evento `GamePurchased` enviado pelo microsserviço de Games.
2. Verifica se o usuário possui saldo suficiente na tabela `Wallet`.
3. Se houver saldo: Deduz o valor e publica o evento `PaymentResult` com sucesso (`Success = true`).
4. Se não houver saldo: Publica o evento `PaymentResult` com falha (`Success = false`) e motivo.

## Execução Local (Docker)

```bash
docker build -t fcg-payments .
docker run -p 8081:80 -e "ConnectionStrings:ServiceBusConnection=SUA_STRING" -e "ConnectionStrings:DefaultConnection=SUA_DB_STRING" fcg-payments