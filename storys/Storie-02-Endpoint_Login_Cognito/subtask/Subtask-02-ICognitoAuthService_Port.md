# Subtask 02: Criar ICognitoAuthService (Port)

## Descrição
Criar a interface `ICognitoAuthService` no projeto Application (Ports) definindo o contrato para operações de autenticação com Cognito, incluindo método `LoginAsync` que será implementado na camada Infra.

## Passos de Implementação
1. Criar arquivo `ICognitoAuthService.cs` em `Application/Ports/`
2. Definir interface com método:
   ```csharp
   Task<LoginOutput> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
   ```
3. Adicionar XML comments documentando:
   - Propósito da interface (abstração para serviço de autenticação Cognito)
   - Parâmetros do método LoginAsync
   - Retorno (LoginOutput com tokens ou null/exceção em caso de erro)
   - Exceções esperadas (ex.: UnauthorizedAccessException para credenciais inválidas)
4. Nota: LoginOutput será criado na Subtask 04; por enquanto, usar tipo temporário ou criar stub vazio

## Formas de Teste
1. Compilar projeto Application e verificar que interface é criada sem erros
2. Criar mock da interface em teste unitário temporário usando Moq e verificar que pode ser mockado corretamente
3. Inspecionar código e confirmar assinatura do método (async Task, CancellationToken, parâmetros corretos)
4. Verificar que interface está no namespace correto (`VideoProcessing.Auth.Application.Ports`)

## Critérios de Aceite da Subtask
- [ ] Interface `ICognitoAuthService` criada em `Application/Ports/ICognitoAuthService.cs`
- [ ] Método `LoginAsync` definido com assinatura correta: `Task<LoginOutput> LoginAsync(string username, string password, CancellationToken cancellationToken = default)`
- [ ] XML comments adicionados documentando propósito, parâmetros, retorno e exceções
- [ ] Interface pode ser mockada em testes (verificado com Moq.Mock<ICognitoAuthService>)
- [ ] `dotnet build` executa sem erros no projeto Application
