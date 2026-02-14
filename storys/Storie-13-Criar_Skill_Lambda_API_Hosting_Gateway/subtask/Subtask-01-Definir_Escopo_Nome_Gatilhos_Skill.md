# Subtask 01: Definir escopo, nome e gatilhos da skill

## Descrição
Definir o nome da skill, o escopo (o que ela cobre e o que não cobre) e os gatilhos (palavras-chave / contextos) que indicam quando o agente deve usar esta skill.

## Passos de implementação
1. Escolher nome da skill (ex.: `lambda-api-hosting`, `aws-lambda-api-net`) e criar pasta `.cursor/skills/<nome-skill>/` com arquivo `SKILL.md` em branco ou com esqueleto.
2. Na descrição (frontmatter e primeira seção), deixar explícito: (a) foco em API .NET 10 com AddAWSLambdaHosting para uso como API com endpoints; (b) combinação com outras rules/skills para arquitetura; (c) objetivo de economizar tempo com configuração de gateway e documentação OpenAPI (Scalar ou Swagger).
3. Listar gatilhos: ex. "criar nova lambda api", "AddAWSLambdaHosting", "API Gateway", "lambda com endpoint", "hospedar API no Lambda", "GATEWAY_PATH_PREFIX", "GATEWAY_STAGE".
4. Documentar o que a skill **não** cobre: decisões de Clean Architecture, camadas, uso de UseCases/Controllers (referenciar core-clean-architecture e core-dotnet).

## Formas de teste
- Ler o SKILL.md e verificar se um desenvolvedor entende quando usar a skill.
- Simular pergunta "quero criar uma nova API .NET para rodar no Lambda" e verificar se os gatilhos levariam à skill.
- Verificar se a distinção entre "bootstrap + gateway" e "arquitetura" está clara.

## Critérios de aceite da subtask
- [ ] Pasta `.cursor/skills/<nome-skill>/` existe com `SKILL.md` contendo frontmatter (name, description).
- [ ] Descrição da skill indica uso para API .NET com AddAWSLambdaHosting e economia de tempo com gateway e documentação OpenAPI (Scalar ou Swagger).
- [ ] Seção "Quando Usar Esta Skill" ou equivalente lista gatilhos claros (mínimo 4).
- [ ] Escopo explícito: não cobre arquitetura de aplicação; combina com outras rules/skills.
