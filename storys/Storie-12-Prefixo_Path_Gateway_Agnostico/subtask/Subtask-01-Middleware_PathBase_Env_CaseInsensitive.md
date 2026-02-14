# Subtask 01: Criar middleware PathBase com env e comparação case-insensitive

## Descrição
Criar um middleware que leia a variável de ambiente `GATEWAY_PATH_PREFIX`. Se estiver vazia ou não definida, não alterar o request. Se estiver preenchida, verificar se o path da requisição começa com esse prefixo (comparação case-insensitive); em caso positivo, definir `Request.PathBase` com o segmento correspondente do path e `Request.Path` com o restante.

## Passos de implementação
1. Criar classe `GatewayPathBaseMiddleware` em `src/VideoProcessing.Auth.Api/Middleware/`.
2. Ler `GATEWAY_PATH_PREFIX` via `Environment.GetEnvironmentVariable("GATEWAY_PATH_PREFIX")` ou IConfiguration; normalizar (trim; remover trailing slash do valor configurado para comparação).
3. Se o valor for null ou vazio (após trim), chamar `_next(context)` sem alterar Path/PathBase.
4. Se preenchido: obter `context.Request.Path`; comparar início do path com o prefixo em modo case-insensitive (ex.: `StartsWith(prefix, StringComparison.OrdinalIgnoreCase)`). Se não começar com o prefixo, não alterar. Se começar, definir `PathBase` com o prefixo real do request (mesmo segmento, preservando casing do request) e `Path` com o restante.
5. Tratar edge cases: path vazio ou "/"; prefixo com ou sem barra inicial; garantir que Path restante comece com "/".

## Formas de teste
1. Teste unitário: env vazia → path não alterado.
2. Teste unitário: env = "/auth", path = "/auth/health" → PathBase="/auth", Path="/health".
3. Teste unitário: env = "/auth", path = "/Auth/health" ou "/AUTH/login" → strip aplicado (case-insensitive).
4. Executar API localmente sem definir a variável; chamar GET /health e GET /auth/login e verificar que o comportamento é o atual.

## Critérios de aceite da subtask
- [ ] Middleware criado; quando `GATEWAY_PATH_PREFIX` não está definida ou está vazia, Path e PathBase não são alterados.
- [ ] Quando definida, o prefixo é removido do path de forma case-insensitive e PathBase/Path são definidos corretamente.
- [ ] Path restante sempre começa com "/"; PathBase reflete o prefixo presente no request (casing do request).
