# Extractt

## Descrição
Extractt é um serviço destinado a extração de textos de documentos. Seu funcionando é de forma assíncrona e seu cliente não precisa aguardar o processamento para ter o resultado, o resultado é enviado assim que o processamento termina atavés de uma url de callback.

![alt text](https://github.com/StephanyBatista/extractt/blob/master/Imgs/extractt_flow.png?raw=true)

## Por que Extractt?
Com o Extractt evitamos fazer loppings infinitos para descobrir se no sistema cliente existe um novo arquivo e a partir disso iniciar a extração do texto de tal documento. Com a abordagem de loop, sobrecarregamos ou o sistema cliente ou seu banco de dados, sem contar que muitas vezes podemos utilizar a rede e os recursos de forma desnecessária quando não um novo documento a ser processado.

Outro motivo que faz o uso do Extractt é a fault-tolerant, já que em caso de falha do sistema cliente em disponbilizar o documento ou no processamento do Extractt, jobs são executados novamente.

## Quais ferramentas utiliza
Extracct utiliza duas ferramentas para a extração de textos. 

#### PdfToText
PdfToText é uma ferramenta para extração de textos do PDF que nasceram de forma digital. Essa ferramenta deve ser instalada no SO de onde será executado o Extractt.

#### Cognitive Services
Cognitive Services é uma ferramenta do Azure que tem a função de extrair textos do PDF onde estes foram scaneados e não foi possível ser extraido através da ferramenta PdfToText.

#### Hangfire
Hangfire é uma biblioteca C# para gerenciamento de jobs na aplicação, com ela podemos agendar jobs sem nos preocupar com sua segurança, já que ela faz o uso do armazenamento dos status dos jobos no SQL Server para em caso de falha da aplicação.

#### SQL Server
Banco de dados utilizado para armazenar os jobs criados pelo Hangfire.


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

Abaixo um exemplo de 10 tentativas feitas pelo Hangfire

```
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 1 of 10 will be performed in 00:00:44.
System.Exception: Error to callback
  
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 2 of 10 will be performed in 00:00:18.
System.Exception: Error to callback
   
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 3 of 10 will be performed in 00:01:07.
System.Exception: Error to callback
   
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 4 of 10 will be performed in 00:01:36.
System.Exception: Error to callback
   
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 5 of 10 will be performed in 00:06:46.
System.Exception: Error to callback
   
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 6 of 10 will be performed in 00:12:58.
System.Exception: Error to callback

warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 7 of 10 will be performed in 00:22:47.
System.Exception: Error to callback
   
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 8 of 10 will be performed in 00:42:24.
System.Exception: Error to callback
  
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 9 of 10 will be performed in 01:11:13.
System.Exception: Error to callback
   
warn: Hangfire.AutomaticRetryAttribute[0]
      Failed to process the job '72': an exception occurred. Retry attempt 10 of 10 will be performed in 01:49:46.
System.Exception: Error to callback
   
```


## Inicialização
##### Banco de dados
Para que o Extractt possa ser escalável e seguro a ponto de não perder nenhum job criado em caso de pane, um banco de dados SQL Server deverá ser utilizado para armazenamento dos eventos. 

O startup do projeto já cria os objetos necessários no banco de dados. Para isso é necessário que o banco de dados já esteja criado e crie a variável de ambiente do banco de dados contendo a string de conexão.
```
export HANGFIRE_CONNECTION="Server=localhost;Database=Extractt;User Id=sa;Password=p4ssw0rd*;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES"
```

##### Cognitive services
Para que o Extractt possa utilizar o Cognitive Services é necessário que a variável de ambiente seja criada conforme abaixo
```
export COGNITIVE_API={URL_COGNITIVE}
export COGNITIVE_KEY={KEY_COGNITIVE}
```

As duas variáveis são necessárias para a utilização do Cognitive Services.



