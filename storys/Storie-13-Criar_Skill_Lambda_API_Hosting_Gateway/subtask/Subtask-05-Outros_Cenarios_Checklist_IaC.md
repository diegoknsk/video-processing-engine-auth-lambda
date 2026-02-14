# Subtask 05: Outros cenários e checklist IaC (Handler, timeout, evento v2)

## Descrição
Cobrir na skill outros cenários que causam erros comuns: Handler correto (nome do assembly), timeout mínimo recomendado (cold start), formato de evento HTTP API v2 para testes no console da Lambda; checklist breve para IaC (Terraform/CloudFormation/GitHub Actions).

## Passos de implementação
1. **Handler:** Com AddAWSLambdaHosting e sem LambdaEntryPoint, o Handler na AWS deve ser apenas o **nome do assembly** (ex.: `VideoProcessing.Auth.Api` ou `MinhaApi`). Alertar para não usar formato Assembly::Classe::Método a menos que exista classe de entrada.
2. **Timeout:** Recomendar mínimo **30 segundos** (cold start .NET pode levar 5–15+ s); mencionar que o padrão de 3 s causa Sandbox.Timedout.
3. **Memória:** Sugerir 512 MB ou mais para melhor cold start.
4. **Evento de teste (Console Lambda):** Com LambdaEventSource.HttpApi, o evento deve ser **API Gateway HTTP API v2**; não usar template "Hello World". Incluir ou referenciar exemplo de evento JSON para GET /health.
5. **Checklist IaC:** Runtime .NET 10; Handler = nome do assembly; Timeout ≥ 30 s; variáveis GATEWAY_PATH_PREFIX e GATEWAY_STAGE quando aplicável; .zip com saída de dotnet publish (arquivos na raiz do zip).

## Formas de teste
- Validar que um desenvolvedor seguindo a skill evita os erros descritos em `docs/lambda-handler-addawslambdahosting.md`.
- Conferir se o checklist cobre os itens do resumo da seção 6 do lambda-handler-addawslambdahosting.md.

## Critérios de aceite da subtask
- [ ] Handler documentado: nome do assembly quando AddAWSLambdaHosting sem LambdaEntryPoint.
- [ ] Timeout mínimo 30 s e memória sugerida (ex.: 512 MB) documentados.
- [ ] Formato de evento HTTP API v2 para testes no console mencionado com exemplo ou referência.
- [ ] Checklist IaC com: Runtime, Handler, Timeout, variáveis de ambiente, estrutura do .zip.
