# Subtask 06: Redação final SKILL.md e referência em rules se aplicável

## Descrição
Revisar a skill completa, garantir consistência de redação e links internos; adicionar referência à nova skill nas rules ou na documentação do projeto quando fizer sentido (ex.: regra que lista skills por contexto).

## Passos de implementação
1. Revisar todo o conteúdo do SKILL.md: ortografia, clareza, ordem das seções, exemplos de código alinhados ao .NET 10.
2. Garantir que links para documentação do projeto (docs/gateway-path-prefix.md, docs/lambda-handler-addawslambdahosting.md) usem caminhos relativos ou absolutos corretos conforme uso (skill pode ser copiada para outros repositórios; nesse caso considerar referência genérica ou "ver documentação do projeto de referência").
3. Verificar nas rules existentes (core-dotnet, core-clean-architecture) se há tabela "Quando Usar Skills Especializadas"; se houver, adicionar linha para a nova skill com gatilhos (ex.: "Lambda API com AddAWSLambdaHosting, API Gateway, GATEWAY_PATH_PREFIX").
4. Opcional: adicionar em docs/ (ex.: contexto-arquitetural.md ou novo doc) menção à skill para novos lambdas.

## Formas de teste
- Ler a skill de ponta a ponta como se fosse um novo desenvolvedor; verificar se consegue seguir sem ambiguidade.
- Procurar por "lambda", "gateway", "AddAWSLambdaHosting" nas rules e confirmar que a nova skill está referenciada onde apropriado.

## Critérios de aceite da subtask
- [ ] SKILL.md revisado: seções coerentes, exemplos válidos, sem contradições.
- [ ] Referências a docs do repositório ou a "projeto de referência" consistentes.
- [ ] Se existir tabela de skills nas rules, nova skill incluída com gatilhos adequados.
- [ ] Documentação do projeto atualizada com menção à skill quando aplicável (opcional).
