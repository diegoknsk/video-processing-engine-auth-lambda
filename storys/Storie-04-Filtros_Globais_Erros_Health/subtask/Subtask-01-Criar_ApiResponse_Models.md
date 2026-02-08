# Subtask 01: Criar Modelos ApiResponse e ApiErrorResponse

## Descrição
Criar as classes `ApiResponse<T>` e `ApiErrorResponse` no projeto Api para padronizar o formato de todas as respostas da API (sucesso e erro), incluindo timestamp UTC e campos apropriados.

## Passos de Implementação
1. Criar classe `ApiResponse<T>` em `Api/Models/ApiResponse.cs`:
   ```csharp
   public class ApiResponse<T>
   {
       public bool Success { get; init; } = true;
       public T? Data { get; init; }
       public DateTime Timestamp { get; init; } = DateTime.UtcNow;
   }
   ```
2. Criar classe `ApiErrorResponse` em `Api/Models/ApiErrorResponse.cs`:
   ```csharp
   public class ApiErrorResponse
   {
       public bool Success { get; init; } = false;
       public ErrorDetail Error { get; init; }
       public DateTime Timestamp { get; init; } = DateTime.UtcNow;
   }
   
   public class ErrorDetail
   {
       public string Code { get; init; }
       public string Message { get; init; }
   }
   ```
3. Adicionar XML comments documentando cada propriedade
4. Considerar usar `record` em vez de `class` para imutabilidade (decisão de design)
5. Adicionar construtores ou métodos estáticos de criação se necessário (ex.: `ApiResponse<T>.Success(T data)`, `ApiErrorResponse.Create(string code, string message)`)

## Formas de Teste
1. Criar testes unitários simples que instanciam os modelos e verificam propriedades
2. Testar serialização JSON dos modelos (usar `System.Text.Json.JsonSerializer.Serialize`) e verificar formato esperado (camelCase, campos corretos)
3. Compilar projeto Api e verificar que modelos são criados sem erros
4. Usar modelos temporariamente em controller dummy e verificar resposta JSON

## Critérios de Aceite da Subtask
- [ ] Classe `ApiResponse<T>` criada com propriedades Success (bool), Data (T nullable), Timestamp (DateTime UTC)
- [ ] Classe `ApiErrorResponse` criada com propriedades Success (bool, default false), Error (ErrorDetail), Timestamp (DateTime UTC)
- [ ] Classe `ErrorDetail` criada com propriedades Code (string) e Message (string)
- [ ] XML comments adicionados documentando cada propriedade
- [ ] Modelos serializam corretamente para JSON (camelCase); testado com JsonSerializer
- [ ] `dotnet build` executa sem erros
