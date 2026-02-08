# Subtask 05: Criar Documentação de Geração de Client Kiota

## Descrição
Criar documento `docs/kiota-client-generation.md` com instruções completas para gerar client C# tipado a partir da especificação OpenAPI usando Kiota CLI, incluindo instalação do Kiota, comando de geração, onde salvar o client, e exemplo de uso básico.

## Passos de Implementação
1. Criar arquivo `docs/kiota-client-generation.md` com as seguintes seções:
   - **Pré-requisitos:** instalação do Kiota CLI
     ```bash
     dotnet tool install --global Microsoft.OpenApi.Kiota
     ```
   - **Gerar client C#:** comando completo
     ```bash
     kiota generate \
       --openapi https://api.example.com/openapi/v1.json \
       --language CSharp \
       --class-name VideoProcessingAuthClient \
       --namespace VideoProcessing.Clients.Auth \
       --output ./clients/VideoProcessing.Clients.Auth
     ```
     (Ajustar URL do OpenAPI conforme ambiente: local, dev, prod)
   - **Onde salvar:** sugestão de estrutura de pastas (ex.: `clients/` na raiz do repositório, ou repositório separado de clients se houver múltiplos serviços)
   - **Uso básico do client gerado:** exemplo de código
     ```csharp
     var client = new VideoProcessingAuthClient(new HttpClient { BaseAddress = new Uri("https://api.example.com") });
     var loginResponse = await client.Auth.Login.PostAsync(new LoginRequest { Username = "user", Password = "pass" });
     Console.WriteLine($"Access Token: {loginResponse.AccessToken}");
     ```
   - **Notas importantes:**
     - Atualizar client sempre que OpenAPI mudar (comando `kiota update`)
     - Client gerado é type-safe e facilita consumo da API
     - Considerar versionamento do client (ex.: tags no Git alinhadas às versões da API)
2. Adicionar link no `README.md` principal para o documento de Kiota
3. (Opcional) Incluir seção sobre outras linguagens suportadas por Kiota (TypeScript, Python, Java, Go)

## Formas de Teste
1. Revisar documento e verificar clareza, completude e exemplos funcionais
2. (Opcional) Executar comando de geração localmente (se Kiota CLI estiver instalado) e verificar que client é gerado sem erros
3. (Opcional) Usar client gerado em projeto de teste e verificar que chamadas à API funcionam
4. Verificar que link no README.md aponta corretamente para o documento de Kiota

## Critérios de Aceite da Subtask
- [ ] Arquivo `docs/kiota-client-generation.md` criado com instruções completas (instalação, comando de geração, onde salvar, exemplo de uso)
- [ ] Comando de geração inclui todos os parâmetros necessários (openapi, language, class-name, namespace, output)
- [ ] Exemplo de uso básico do client gerado está presente e correto
- [ ] Notas importantes sobre atualização de client e versionamento incluídas
- [ ] Link adicionado no `README.md` para o documento de Kiota
- [ ] (Opcional) Menção a outras linguagens suportadas por Kiota
- [ ] Documento revisado e livre de erros gramaticais ou técnicos
