# Extractt

## Descrição
Extractt is a service for extracting texts from documents. Its functioning is asynchronous and your client does not need to wait for processing to have the result. When the service finishi the processing, the client receives the result from extract throught the callback url sent by client when asked in the beginning.

![alt text](https://github.com/StephanyBatista/extractt/blob/master/Assets/extractt_flow.png?raw=true)

## Why use Extractt?
Extracting text from documents is usually time consuming, on average 30s per page for documents that were not born digitally. Preventing the end user from being penalized for this delay and removing heavy processes from the client system are the goals for creating Extractt.

Fault-tolerant is another important reason for a process like this, restarting at the point of the crash is also important so that the client does not run out of the indexed document.

## Which tools does Extractt use?
Extractt uses some tools to extract texts. 

#### PdfToText
PdfToText is a tool for extracting texts from digital PDF files. This tool must be installed on the OS from which Extractt will run.

#### Qpdf
Qpdf is a tool to generate the pdf page separately.

#### Cognitive Services
Cognitive Services is an Azure tool that has the function of extracting text from images. For this to happen, Extractt transforms all pages to jpg and sends them separately to Cognitive for processing. This tool is paid, it is necessary to pass the API and Key parameters for Extractt to use correctly.

#### Pdftoppm
Pdftoppm is a tool for converting pdf to image and thus sending it to Cognitive Services.

All of these tools must be installed on the machine that will be used to run the application. In the case of windows, these tools are already in the "dependencies-win" folder and nothing needs to be done.


## How was the extract developed?

Below are shown all tools for building the Extractt

#### ASP.NET Core
ASP.NET Core is a Web Framework from Microsft extremely fast.

#### Hangfire
Hangfire is a C# library for job management in the application, with it we can schedule jobs without worrying about their security, since it makes use of job status storage in SQL Server / Memory for in case of application failure.

#### SQL Server (Not obligatory)
Data base used to store jobs created. 

## Inicialização
##### Banco de dados
In order for Extractt to be scalable and secure to the point of not losing any scheduled jobs in the event of a failure, a SQL Server database must be used to store the events.

There is a file called migration.sql and this file must be used to create database objects necessary to Hangfire uses.

In order to inform the connection string to Extractt, the environment variable must be used. An example is used below.
```
export HANGFIRE_CONNECTION="Server=localhost;Database=Extractt;User Id=sa;Password=p4ssw0rd*;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES"
```

NOTE: The use of the database is not mandatory. Not set the  environment variables, makes the whole process in memory (Not advised in production).

##### Hangfire
Hangifire has a dasboard where is possible see, re-work and delete jobs created by the application. Because of that, an user must be configured by enrironment variable like below.
```
export HANGFIRE_USER=user
export HANGFIRE_PASSWORD=123456
```

##### Cognitive services
In order to Extractt to be able to use Cognitive Services it is necessary that the environment variables be created as shown below.
```
export COGNITIVE_API={URL_COGNITIVE}
export COGNITIVE_KEY={KEY_COGNITIVE}
```

##### Segurança
An access key must be informed to Extractt and it is used by client systems for this call to be made securely. If the client system passes an incorrect access key, Extractt will refuse the call from client
```
export ACCESS_KEY=aaÇÇ34567
```

## Como funciona?
The extractt service client accesses the POST api/process api with the following payload
```
{
	"URL": "https://gahp.net/wp-content/uploads/2017/09/sample.pdf",
	"CallbackUrl": "https://webhook.site/8678864d-5ac9-43ed-a462-c744e5890043aaabbb",
	"DocumentIdentifier": "XPTO",
	"AccessKey": "aaÇÇ34567"
}	
```
- Url is the link of the document to be downloaded and processed by Extractt.
- CallbackUrl is the link of the client system that Extractt will answer after processing the document.
- DocumentIdentifier is the parameter that the extractt will send to the client system so that it identifies the document informed.
- AccessKey is the access key that Extractt needs when starting the system. This key must be kept secret by the client system.

Extractt when receiving the payload, schedules a job to process the document and returns the status 200 to the customer if all parameters are correct. 

After the entire job is executed, another job is created to call the callback url (PUT {CallbackUrlFromClient}) of the client system with the result in json.
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
The client system will receive the json above.
- Success informs you of the processing success.
- ErrorMessage informs the error description if it exists
- Pages informs the text extracted per page.


