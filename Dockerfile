FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestFunctionApp.csproj", "."]
RUN dotnet restore "./TestFunctionApp.csproj"
COPY . .
RUN dotnet build "TestFunctionApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestFunctionApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true