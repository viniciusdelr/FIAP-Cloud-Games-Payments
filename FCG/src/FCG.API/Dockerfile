# ----------------------------
# Step 1: Build
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia a solução e todos os projetos necessários
COPY FCG-Payments.sln ./
COPY src/FCG.API/ ./FCG.API/
COPY src/FCG.Application/ ./FCG.Application/
COPY src/FCG.Domain/ ./FCG.Domain/
COPY src/FCG.Infrastructure/ ./FCG.Infrastructure/

# Restaura dependências do projeto principal
RUN dotnet restore FCG.API/FCG.API.csproj

# Publica o projeto principal
RUN dotnet publish FCG.API/FCG.API.csproj -c Release -o /app/publish

# ----------------------------
# Step 2: Runtime
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
#
## Instala dependências do New Relic
#RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
    #&& echo 'deb [signed-by=/usr/share/keyrings/newrelic-apt.gpg] http://apt.newrelic.com/debian/ newrelic non-free' \
       #| tee /etc/apt/sources.list.d/newrelic.list \
    #&& wget -O- https://download.newrelic.com/NEWRELIC_APT_2DAD550E.public \
       #| gpg --import --batch --no-default-keyring --keyring /usr/share/keyrings/newrelic-apt.gpg \
    #&& apt-get update \
    #&& apt-get install -y newrelic-dotnet-agent \
    #&& rm -rf /var/lib/apt/lists/*
#
## Configuração do New Relic
#ENV CORECLR_ENABLE_PROFILING=1 \
    #CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    #CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
    #CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    #NEW_RELIC_APP_NAME=FCGAPI-FIAP
#
#ARG NEW_RELIC_LICENSE_KEY
#ENV NEW_RELIC_LICENSE_KEY=$NEW_RELIC_LICENSE_KEY

# Copia app publicado do stage build
COPY --from=build /app/publish .

# Expõe porta
EXPOSE 80

# EntryPoint da API
ENTRYPOINT ["dotnet", "FCG.API.dll"]