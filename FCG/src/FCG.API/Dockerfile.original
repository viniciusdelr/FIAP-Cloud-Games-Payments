# step 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia tudo de uma vez (garante que os .csproj e arquivos estejam no lugar)
COPY . .

# Restaura
RUN dotnet restore FCG.sln

# Publica
RUN dotnet publish src/FCG.API/FCG.API.csproj -c Release -o /app/publish

# step 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "FCG.API.dll"]
