# Subtask 06: Configurar FluentValidation

## Descrição
Configurar FluentValidation no projeto Api com assembly scanning automático, auto-validação de ModelState, e filtro global para converter erros de validação no formato padronizado da API.

## Passos de Implementação
1. No `Program.cs` do projeto Api, adicionar após `AddControllers()`:
   - `builder.Services.AddFluentValidationAutoValidation();`
   - `builder.Services.AddFluentValidationClientsideAdapters();`
   - `builder.Services.AddValidatorsFromAssemblyContaining<Program>();` (irá escanear assembly da Application quando houver validators)
2. Criar filtro `ValidationFilter` em `Api/Filters/` que:
   - Implementa `IActionFilter`
   - No `OnActionExecuting`, verifica `ModelState.IsValid`
   - Se inválido, retorna `BadRequest` com formato `{ "success": false, "errors": [...] }`
3. Registrar o filtro globalmente: `builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>());`
4. Criar validator de exemplo (dummy) no Application para testar: `DummyInputValidator : AbstractValidator<DummyInput>` com regra simples (ex.: `RuleFor(x => x.Name).NotEmpty()`)
5. Executar `dotnet build` e verificar que FluentValidation está registrado sem erros

## Formas de Teste
1. Criar endpoint dummy temporário no Api que aceita DummyInput e executar request inválido (campo vazio) via curl/Postman
2. Verificar que resposta é 400 Bad Request com formato `{ "success": false, "errors": [{ "field": "...", "message": "..." }] }`
3. Executar teste unitário do ValidationFilter mockando ModelState inválido e verificando resposta
4. Confirmar que validators são descobertos automaticamente (log no startup ou breakpoint no validator)

## Critérios de Aceite da Subtask
- [ ] FluentValidation registrado com `AddFluentValidationAutoValidation()` e assembly scanning no `Program.cs`
- [ ] Filtro `ValidationFilter` criado e registrado globalmente em Controllers
- [ ] Filtro converte erros de ModelState para formato padronizado `{ "success": false, "errors": [...] }`
- [ ] Validator dummy criado e testado; validação automática funciona (400 retornado quando input inválido)
- [ ] `dotnet build` executa sem erros
- [ ] Teste unitário do ValidationFilter passando com cobertura adequada
