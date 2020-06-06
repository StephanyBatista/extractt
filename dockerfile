FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY /Web ./src/app
WORKDIR /src/app
RUN dotnet restore
RUN dotnet build
RUN dotnet publish -c Release -o /src/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.2-alpine3.11 AS runtime
COPY --from=build /src/out ./app
RUN apk update && apk add poppler-utils qpdf bash
WORKDIR /app
ENTRYPOINT ["dotnet", "Extractt.Web.dll"]