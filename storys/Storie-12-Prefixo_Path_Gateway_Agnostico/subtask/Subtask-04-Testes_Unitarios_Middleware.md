# Subtask 04: Testes unitários do middleware

## Descrição
Implementar testes unitários para o GatewayPathBaseMiddleware cobrindo: env vazia/não definida (path inalterado); env preenchida com path que começa com o prefixo (PathBase e Path definidos); comparação case-insensitive; path que não começa com o prefixo (inalterado).

## Passos de implementação
1. No projeto de testes (ex.: VideoProcessing.Auth.Tests.Unit), criar classe de teste para o middleware (ex.: GatewayPathBaseMiddlewareTests).
2. Cenários: (a) GATEWAY_PATH_PREFIX não definida → Path e PathBase inalterados; (b) GATEWAY_PATH_PREFIX="/auth", path="/auth/health" → PathBase="/auth", Path="/health"; (c) path="/Auth/login" ou "/AUTH/users/create" → strip aplicado (case-insensitive); (d) path="/other/health" com prefix "/auth" → inalterado.
3. Usar DefaultHttpContext, definir Request.Path e (se necessário) mock ou setar variável de ambiente antes do teste e limpar após (ou usar IConfiguration mockado se o middleware ler de IConfiguration).
4. Executar `dotnet test` e garantir que todos os testes passam.

## Formas de teste
1. Executar `dotnet test` no projeto de testes unitários.
2. Verificar cobertura do middleware (opcional).

## Critérios de aceite da subtask
- [ ] Testes unitários cobrem: env vazia, env com prefixo e path correspondente, case-insensitive, path sem prefixo.
- [ ] `dotnet test` passa para o projeto VideoProcessing.Auth (solução).
- [ ] Testes são determinísticos (não depender de ordem de execução; limpar env após cada teste se necessário).
