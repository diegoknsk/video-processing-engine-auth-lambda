# Subtask 03: Registrar middleware e documentar variável GATEWAY_PATH_PREFIX

## Descrição
Registrar o middleware no pipeline do `Program.cs` como primeiro middleware (antes do GlobalExceptionMiddleware ou logo no início) e documentar a variável de ambiente e o comportamento em documentação do projeto.

## Passos de implementação
1. No `Program.cs`, adicionar `app.UseMiddleware<VideoProcessing.Auth.Api.Middleware.GatewayPathBaseMiddleware>()` como primeiro middleware do pipeline (antes de UseMiddleware<GlobalExceptionMiddleware> ou no topo após builder.Build()).
2. Criar ou atualizar documento em `docs/` (ex.: `docs/gateway-path-prefix.md` ou seção em `docs/processo-subida-deploy.md` / `docs/lambda-handler-addawslambdahosting.md`) descrevendo:
   - Nome da variável: `GATEWAY_PATH_PREFIX`
   - Comportamento: não definida ou vazia = path inalterado; definida = prefixo removido do path (case-insensitive), PathBase definido
   - Exemplo de uso no API Gateway (ex.: /auth) e em IaC (Terraform/CloudFormation) definindo a variável na Lambda
3. Opcional: mencionar no README.md a existência da variável para deploy atrás de gateway com prefixo.

## Formas de teste
1. Build e execução da API; verificar que o middleware está na ordem correta (primeira chamada no pipeline).
2. Ler a documentação e confirmar que um novo desenvolvedor entenderia como usar a variável em produção.

## Critérios de aceite da subtask
- [ ] Middleware registrado como primeiro no pipeline em Program.cs.
- [ ] Documentação descreve `GATEWAY_PATH_PREFIX`, comportamento (vazio vs preenchido) e comparação case-insensitive.
- [ ] Build do projeto passa; API inicia sem erros.
