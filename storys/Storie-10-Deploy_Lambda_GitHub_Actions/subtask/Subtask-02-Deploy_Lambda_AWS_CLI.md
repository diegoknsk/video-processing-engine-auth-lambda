# Subtask 02: Implementar Step de Deploy no Lambda via AWS CLI

## Descrição
Adicionar ao workflow do GitHub Actions os steps necessários para configurar credenciais AWS e fazer o deploy do pacote ZIP gerado no Lambda usando AWS CLI. O deploy deve atualizar o código do Lambda função já provisionada pela infraestrutura.

## Objetivos
1. Adicionar step de configuração de credenciais AWS no workflow
2. Implementar step de deploy que faz upload do ZIP para o Lambda
3. Utilizar secrets do GitHub para credenciais AWS (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY)
4. Utilizar variáveis do GitHub para configuração (AWS_REGION, LAMBDA_FUNCTION_NAME)
5. Validar se o deploy foi bem-sucedido

## Detalhes Técnicos

### Steps de Deploy (adicionar após "Create deployment package")

```yaml
      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v4
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: ${{ env.AWS_REGION }}
      
      - name: Deploy to Lambda
        run: |
          aws lambda update-function-code \
            --function-name ${{ env.LAMBDA_FUNCTION_NAME }} \
            --zip-file fileb://deployment-package.zip \
            --region ${{ env.AWS_REGION }}
      
      - name: Wait for Lambda update to complete
        run: |
          aws lambda wait function-updated \
            --function-name ${{ env.LAMBDA_FUNCTION_NAME }} \
            --region ${{ env.AWS_REGION }}
      
      - name: Verify deployment
        run: |
          echo "Lambda function deployed successfully!"
          aws lambda get-function \
            --function-name ${{ env.LAMBDA_FUNCTION_NAME }} \
            --region ${{ env.AWS_REGION }} \
            --query 'Configuration.[FunctionName,LastModified,Runtime,CodeSize]' \
            --output table
```

### Explicação dos Steps

1. **Configure AWS credentials**: Configura credenciais AWS usando action oficial, lê secrets do GitHub
2. **Deploy to Lambda**: Executa `aws lambda update-function-code` para fazer upload do ZIP
3. **Wait for Lambda update**: Aguarda o Lambda finalizar a atualização (importante para deploys grandes)
4. **Verify deployment**: Exibe informações do Lambda após deploy para validação

### Secrets Necessários (GitHub Secrets)

- `AWS_ACCESS_KEY_ID`: Access Key ID do IAM User/Role com permissões de deploy
- `AWS_SECRET_ACCESS_KEY`: Secret Access Key correspondente

### Variáveis Necessárias (GitHub Variables)

- `AWS_REGION`: Região AWS (padrão: us-east-1)
- `LAMBDA_FUNCTION_NAME`: Nome da função Lambda (padrão: video-processing-engine-dev-auth)

### Permissões IAM Necessárias

O IAM User/Role usado deve ter as seguintes permissões:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "lambda:UpdateFunctionCode",
        "lambda:GetFunction",
        "lambda:GetFunctionConfiguration"
      ],
      "Resource": "arn:aws:lambda:REGION:ACCOUNT_ID:function/video-processing-engine-dev-auth"
    }
  ]
}
```

## Passos de Implementação

1. Adicionar step de configuração AWS credentials após criação do ZIP
2. Adicionar step de deploy usando `aws lambda update-function-code`
3. Adicionar step de wait para aguardar conclusão do deploy
4. Adicionar step de verificação para exibir informações do Lambda
5. Documentar secrets e variáveis necessários (será detalhado na Subtask 04)

## Critérios de Aceite

- [ ] Step "Configure AWS credentials" implementado usando action `aws-actions/configure-aws-credentials@v4`
- [ ] Step "Deploy to Lambda" implementado com comando `aws lambda update-function-code`
- [ ] Step "Wait for Lambda update" implementado para garantir conclusão do deploy
- [ ] Step "Verify deployment" implementado para exibir informações do Lambda
- [ ] Workflow utiliza `secrets.AWS_ACCESS_KEY_ID` e `secrets.AWS_SECRET_ACCESS_KEY`
- [ ] Workflow utiliza variáveis `AWS_REGION` e `LAMBDA_FUNCTION_NAME` (com fallback para valores padrão)
- [ ] Deploy atualiza código do Lambda com sucesso
- [ ] Logs do workflow mostram informações de deploy (tamanho do código, última modificação, etc.)

## Notas

- O comando `update-function-code` substitui o código do Lambda mas mantém configurações (variáveis de ambiente, timeout, memory, etc.)
- O `aws lambda wait function-updated` é importante para deploys maiores e evita race conditions
- A action `aws-actions/configure-aws-credentials` é oficial da AWS e recomendada
- As credenciais AWS devem ter permissões mínimas necessárias (princípio do menor privilégio)
- O arquivo ZIP é referenciado como `fileb://deployment-package.zip` (formato do AWS CLI)
