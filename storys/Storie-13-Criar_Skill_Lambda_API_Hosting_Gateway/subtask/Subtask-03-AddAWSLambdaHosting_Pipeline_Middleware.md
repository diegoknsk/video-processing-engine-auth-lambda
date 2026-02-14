# Subtask 03: Instruções AddAWSLambdaHosting, pipeline e middleware Gateway

## Descrição
Incluir na skill as instruções para: (1) registrar AddAWSLambdaHosting(LambdaEventSource.HttpApi) no builder; (2) ordem do pipeline (middleware de gateway **antes** de UseRouting); (3) implementação ou referência do middleware que remove GATEWAY_STAGE e GATEWAY_PATH_PREFIX (case-insensitive, ordem: primeiro stage, depois prefix).

## Passos de implementação
1. Documentar no SKILL.md: `builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);` e que o Handler na AWS deve ser o **nome do assembly** (ex.: `MinhaApi`) quando não há LambdaEntryPoint.
2. Ordem do pipeline: `UseMiddleware<GatewayPathBaseMiddleware>();` **antes** de `UseRouting();` (para que o roteamento use o path já reescrito).
3. Incluir lógica do middleware (ou referência ao código de referência do projeto): ler GATEWAY_STAGE e remover primeiro segmento do path se coincidir; em seguida ler GATEWAY_PATH_PREFIX e definir PathBase/Path quando o path começar com o prefixo; comparação case-insensitive; quando variáveis não definidas ou vazias, não alterar o path.
4. Mencionar que as rotas da aplicação devem ser definidas **sem** prefixo nem stage (ex.: `/health`, `/login`); o gateway adiciona externamente.

## Formas de teste
- Aplicar as instruções em um projeto de exemplo e validar que `/auth/health` atrás do gateway resulta em rota `/health`.
- Verificar que a ordem UseMiddleware antes de UseRouting está explícita e com justificativa (aspnetcore#49454).

## Critérios de aceite da subtask
- [ ] Skill descreve registro de AddAWSLambdaHosting(LambdaEventSource.HttpApi).
- [ ] Ordem do pipeline documentada: GatewayPathBaseMiddleware antes de UseRouting.
- [ ] Comportamento do middleware descrito (stage depois prefix; case-insensitive; variáveis opcionais).
- [ ] Rotas da aplicação agnósticas ao gateway (sem prefixo/stage no código) explicitamente indicadas.
