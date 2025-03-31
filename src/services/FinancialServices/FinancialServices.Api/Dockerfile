# Etapa base usada no runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos .csproj corretamente com base na estrutura
COPY ["services/FinancialServices/FinancialServices.Api/FinancialServices.Api.csproj", "services/FinancialServices/FinancialServices.Api/"]
COPY ["services/FinancialServices/FinancialServices.Application/FinancialServices.Application.csproj", "services/FinancialServices/FinancialServices.Application/"]
COPY ["services/FinancialServices/FinancialServices.Domain/FinancialServices.Domain.csproj", "services/FinancialServices/FinancialServices.Domain/"]
COPY ["services/FinancialServices/FinancialServices.Utils/FinancialServices.Utils.csproj", "services/FinancialServices/FinancialServices.Utils/"]
COPY ["services/FinancialServices/FinancialServices.Infrastructure/FinancialServices.Infrastructure.csproj", "services/FinancialServices/FinancialServices.Infrastructure/"]

# Restaura pacotes
RUN dotnet restore "services/FinancialServices/FinancialServices.Api/FinancialServices.Api.csproj"

# Copia o restante do código
COPY . .

# Compila o projeto
WORKDIR "/src/services/FinancialServices/FinancialServices.Api"
RUN dotnet build "FinancialServices.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FinancialServices.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialServices.Api.dll"]
