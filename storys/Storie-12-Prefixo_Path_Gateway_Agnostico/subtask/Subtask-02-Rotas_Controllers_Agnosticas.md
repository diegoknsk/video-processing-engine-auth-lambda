# Subtask 02: Ajustar rotas dos controllers para path agnóstico

## Descrição
Alterar os controllers de autenticação e usuário para expor rotas sem o prefixo "auth" na rota da aplicação: /login e /users/create. Assim, quando o middleware remover o prefixo do gateway (ex.: /auth), o path restante (/login, /users/create) corresponderá às rotas dos controllers.

## Passos de implementação
1. Em `AuthController`: alterar `[Route("auth")]` para `[Route("login")]` e o action de login para `[HttpPost]` (path final /login), ou manter `[Route("")]` com `[HttpPost("login")]` conforme convenção do projeto.
2. Em `UserController`: alterar `[Route("auth/users")]` para `[Route("users")]` e manter `[HttpPost("create")]` (path final /users/create).
3. Ajustar mensagens de log que referenciam "/auth/login" ou "/auth/users/create" para refletir o path lógico (ex.: "POST /login", "POST /users/create") ou usar o path do request.
4. Revisar comentários/remarks nos controllers e documentação OpenAPI que citem /auth/login ou /auth/users/create; atualizar se necessário para indicar que a URL final depende do gateway (prefixo + path).

## Formas de teste
1. Rodar a API localmente; GET /health, POST /login, POST /users/create devem responder (sem variável de ambiente).
2. Definir GATEWAY_PATH_PREFIX=/auth e chamar (simulando gateway) /auth/health, /auth/login, /auth/users/create; todos devem retornar 200/201 conforme esperado.
3. Verificar Swagger/Scalar: rotas exibidas como /health, /login, /users/create.

## Critérios de aceite da subtask
- [ ] AuthController expõe POST /login (não mais /auth/login na rota da aplicação).
- [ ] UserController expõe POST /users/create (não mais /auth/users/create na rota da aplicação).
- [ ] HealthController permanece com GET /health.
- [ ] Testes manuais e/ou de integração confirmam que os endpoints respondem com e sem prefixo (quando middleware ativo).
