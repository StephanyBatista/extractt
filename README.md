# Extractt

## Descrição
Extractt é um serviço destinado a extração de textos de documentos pdf e seu funcionando é de forma síncrona, fazendo com que o cliente da requisição tenha que aguardar seu processamento final para ter o resultado.

![alt text](https://github.com/StephanyBatista/extractt/blob/master/Assets/extractt_flow.png?raw=true)

## Por que Extractt?
A extração de textos de documentos geralmente é demorada, em média alguns segundos por página. Evitar que o usuário final seja penalisado com essa demora e remover processos pesados do sistema cliente são os objetivos para a criação do Extractt. Mas como?

O Extract possui o mecanismo de executar a extração de cada página do PDF de maneira paralela, reduzindo assim o tempo final de todo documento a ser processado. 

Como API, o Extractt não foi feito para atender um sistema e sim qualquer sistema que necessite desse processamento.

## Quais ferramentas utiliza para extração de texto?
Extracct utiliza duas ferramentas para a extração de textos. 

#### PdfToText
PdfToText é uma ferramenta para extração de textos de arquivos PDF digitais. Essa ferramenta deve ser instalada no SO de onde será executado o Extractt.

#### Cognitive Services
Cognitive Services é uma ferramenta do Azure que tem a função de extrair textos de imagens. Para que isso aconteça, o Extractt transforma todas as páginas para jpg e envia separadamente para o Cognitive processar.

Essa ferramenta é paga e é necessário passar os parâmetro de API e Key para o Extractt

##### Cognitive services
Para que o Extractt possa utilizar o Cognitive Services é necessário que a variáveis de ambiente estejam criadas conforme abaixo.
```
export COGNITIVE_API={URL_COGNITIVE}
export COGNITIVE_KEY={KEY_COGNITIVE}
```

As duas variáveis são necessárias para a utilização do Cognitive Services.

##### Segurança
Uma chave de acesso deve ser informada ao Extractt e este utilizada por sistemas clientes para que essa chamada seja feita de forma segura. Caso o sistema cliente passe uma chave de acesso incorreta, o Extractt irá recusar o processamento do documento informando um status de Bad Request.
```
export ACCESS_KEY=aaÇÇ34567
```

## Como funciona?
O cliente do serviço extractt acessa a api POST api/queue com o seguinte payload
```
{
	"Url": "https://gahp.net/wp-content/uploads/2017/09/sample.pdf",
	"AccessKey": "aaÇÇ34567"
}	
```
- Url é o link do documento a ser baixado e processado pelo Extractt.
- AccessKey é a chave de acesso que o Extractt necessita na inicitalização do sitema. Essa chave deverá ser mantida em segredo pelo sistema cliente.

Extracct ao receber o payload, inicia o processo de extrair o texto do documento em questão.

Após todo o processo ser executado, o cliente final tem a seguinte resposta.
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






