# Subtask 01: Adicionar step de configuração do Handler no workflow

## Descrição
Adicionar ao workflow `.github/workflows/deploy-lambda.yml` um step que configure o Handler da função Lambda com o valor `VideoProcessing.Auth.Api`, executado após o código ser atualizado e o wait de conclusão, para que Lambdas criadas como “casca” tenham o handler correto em todo deploy.

## Passos de implementação
1. Abrir `.github/workflows/deploy-lambda.yml` e localizar o step **Wait for Lambda update to complete**.
2. Inserir um novo step **Update Lambda handler** logo após o wait, condicionado a `github.event_name != 'pull_request'`, que execute:  
   `aws lambda update-function-configuration --function-name ${{ env.LAMBDA_FUNCTION_NAME }} --region ${{ env.AWS_REGION }} --handler "VideoProcessing.Auth.Api"`.
3. Garantir que o step use as mesmas env (LAMBDA_FUNCTION_NAME, AWS_REGION) já definidas no workflow.

## Formas de teste
- Executar o workflow manualmente (workflow_dispatch) ou via push em main e verificar nos logs que o step “Update Lambda handler” roda e conclui com sucesso.
- No Console AWS, abrir a função Lambda → Configuration → General configuration e confirmar que o campo **Handler** está `VideoProcessing.Auth.Api`.
- Fazer um deploy em uma Lambda que tinha outro handler (ou vazio) e confirmar que após o run o handler está correto.

## Critérios de aceite da subtask
- [ ] Step “Update Lambda handler” existe no workflow após “Wait for Lambda update to complete”.
- [ ] O step usa `aws lambda update-function-configuration` com `--handler "VideoProcessing.Auth.Api"`.
- [ ] O step só executa quando não for PR (mesma condição dos demais steps de deploy).
- [ ] Após um deploy, o Handler da função na AWS está definido como `VideoProcessing.Auth.Api`.
