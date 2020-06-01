# Extractt

## Descrição
Extractt é um serviço destinado a extração de textos de documentos. Seu funcionando é de forma assíncrona e seu cliente não precisa aguardar o processamento para ter o resultado, o resultado é enviado assim que o processamento termina atavés de uma url de callback.


## Quais ferramentas utiliza
Extracct utiliza duas ferramentas para a extração de textos. 

### PdfToText
PdfToText é uma ferramenta para extração de textos do PDF que nasceram de forma digital. Essa ferramenta deve ser instalada no SO de onde será executado o Extractt.

### Cognitive Services
Cognitive Services é uma ferramenta do Azure que tem a função de extrair textos do PDF onde estes foram scaneados e não foi possível ser extraido através da ferramenta PdfToText.


## Como funciona?
O cliente do serviço extractt acessa a api POST api/queue com o seguinte payload
```
{
	"DocumentUrl": "https://gahp.net/wp-content/uploads/2017/09/sample.pdf",
	"CallbackUrl": "https://webhook.site/8678864d-5ac9-43ed-a462-c744e5890043aaabbb",
	"Identifier": "XPTO"
}	
```
- DocumentUrl é o link do documento a ser baixado e processado pelo Extractt.
- CallbackUrl é o link do sistema cliente que o Extractt irá acessar após processamento do documento.
- Identifier é o parâmetro que o extractt irá enviar para o sistema cliente como forma de segurança.

Extracct ao receber o payload, agenda um job para processar o documento e retorna ao cliente o status 200. 

Quando o job se iniciar, o texto é extraído por página do documento. Primeira tenta utilizar a ferramenta PdfToText e em caso de falha se inicia o Cognitive Services.

Após todo o job ser executado, um outro job é criado para chamar a url de callback do sistema cliente com o resultado em json.
```
{
  "Sucess": true,
  "ErrorMessage": null,
  "Pages": [
    {
      "Page": 1,
      "Text": "Sample PDF Document Robert Maron Grzegorz Gmdzióski Februaw 20, 1999 "
    },
    {
      "Page": 2,
      "Text": ""
    },
}
```
O sistema cliente irá receber o json acima.
- Success informa do sucesso do processamento.
- ErrorMessage informa a descrição do erro caso exista
- Pages informa o texto extraído por página.

## Falhas
Todo o processo de jobs no Extractt é gerenciado pelo Hangire.

O primeiro job é responsável por baixar e processar todos as páginas do documento. Em caso de falha em qualquer parte o Hangifire re-agenda para fazer mais 10 tentativas, com tempos crescentes após cada tentativa.

O segundo job é responsável por obter o resultado do primeiro job é enviar este para o sistema cliente. A mesma coisa em relação ao primeiro job acontece nesse item em caso de falha.

