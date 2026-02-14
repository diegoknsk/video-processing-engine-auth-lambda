# Prefixo de path do API Gateway (GATEWAY_PATH_PREFIX)

Quando a API Auth roda atrás de um **API Gateway** (HTTP API) que expõe as rotas sob um prefixo (ex.: `/auth`, `/dev/auth`), o path que chega na Lambda inclui esse prefixo (ex.: `rawPath = "/auth/health"`). A aplicação define rotas sem o prefixo (ex.: `/health`, `/login`, `/users/create`), o que geraria 404.

O **middleware** `GatewayPathBaseMiddleware` remove o prefixo configurado do path e define `PathBase` e `Path` para que o roteamento da aplicação funcione sem alterar as rotas no código.

## Variável de ambiente

| Nome | Obrigatória | Descrição |
|------|-------------|-----------|
| `GATEWAY_PATH_PREFIX` | Não | Prefixo de path que o gateway adiciona antes de encaminhar para a Lambda (ex.: `/auth`, `/autenticacao`). |

## Comportamento

- **Não definida ou vazia:** o path do request **não é alterado**. O comportamento é o atual: requisições como `GET /health` e `POST /login` funcionam diretamente. Ideal para execução local e testes sem gateway.
- **Definida (ex.: `/auth`):** se o path da requisição **começar** com esse prefixo, o middleware define:
  - `Request.PathBase` = prefixo presente no request (preserva o casing da requisição)
  - `Request.Path` = restante do path (ex.: `/auth/health` → Path = `/health`)
  - A comparação é **case-insensitive**: `/auth/health`, `/Auth/health` e `/AUTH/health` são tratados da mesma forma.

## Exemplos

| GATEWAY_PATH_PREFIX | Request path   | PathBase (após middleware) | Path (após middleware) |
|--------------------|----------------|----------------------------|-------------------------|
| não definida       | `/auth/health` | (inalterado)               | (inalterado)            |
| não definida       | `/health`      | (inalterado)               | (inalterado)            |
| `/auth`            | `/auth/health` | `/auth`                    | `/health`               |
| `/auth`            | `/Auth/login`  | `/Auth`                    | `/login`                |
| `/auth`            | `/AUTH/users/create` | `/AUTH`              | `/users/create`         |
| `/auth`            | `/other/health`| (inalterado)               | (inalterado)            |

## Uso no deploy (Lambda + API Gateway)

1. No **API Gateway**, configure a rota que encaminha para a Lambda (ex.: `ANY /auth/{proxy+}` ou `GET /auth/health`, `POST /auth/login`, etc.).
2. Na **Lambda**, defina a variável de ambiente `GATEWAY_PATH_PREFIX=/auth` (ou o valor exato do prefixo que o gateway usa no path enviado à Lambda).
3. A aplicação passa a receber o path já “sem” o prefixo para roteamento: `/health`, `/login`, `/users/create`.

Em **Terraform** (exemplo):

```hcl
resource "aws_lambda_function" "auth_api" {
  # ...
  environment {
    variables = {
      GATEWAY_PATH_PREFIX = "/auth"
    }
  }
}
```

Em **CloudFormation** ou **Console da AWS**: adicione a variável de ambiente `GATEWAY_PATH_PREFIX` com valor `/auth` (ou o prefixo desejado) na configuração da função.

## Rotas da aplicação (agnósticas ao gateway)

As rotas expostas pela API são sempre:

- `GET /health` — health check
- `POST /login` — login
- `POST /users/create` — criação de usuário

Localmente, use essas rotas diretamente. Atrás do gateway com prefixo `/auth`, a URL pública será algo como `https://.../auth/health`, `https://.../auth/login`, etc.; internamente o middleware faz o path ser `/health`, `/login`, etc.
