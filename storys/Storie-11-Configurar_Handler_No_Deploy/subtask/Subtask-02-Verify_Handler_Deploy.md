# Subtask 02: Incluir Handler na verificação do deploy

## Descrição
Alterar o step **Verify deployment** do workflow para incluir o **Handler** na saída da consulta `get-function`, permitindo validar nos logs da action que o handler está configurado corretamente após o deploy.

## Passos de implementação
1. No arquivo `.github/workflows/deploy-lambda.yml`, localizar o step **Verify deployment** que executa `aws lambda get-function ... --query 'Configuration.[...]'`.
2. Incluir `Handler` na lista de campos do `--query`, por exemplo:  
   `--query 'Configuration.[FunctionName,LastModified,Runtime,State,Handler]'` (ou a ordem desejada).
3. Manter o `--output table` para que o Handler apareça na tabela exibida nos logs.

## Formas de teste
- Rodar o workflow de deploy e abrir os logs do step “Verify deployment”; conferir que a tabela exibida contém a coluna Handler com valor `VideoProcessing.Auth.Api`.
- Comparar com o valor exibido em Configuration da função no Console AWS.

## Critérios de aceite da subtask
- [ ] O step “Verify deployment” inclui o campo Handler na query de `get-function`.
- [ ] Os logs do workflow mostram o Handler na saída em tabela.
- [ ] O valor exibido para Handler é `VideoProcessing.Auth.Api` após um deploy bem-sucedido.
