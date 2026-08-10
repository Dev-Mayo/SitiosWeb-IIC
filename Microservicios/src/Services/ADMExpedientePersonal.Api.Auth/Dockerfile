# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos solo el proyecto de Auth
COPY . .

# Restauramos dependencias del proyecto específico
RUN dotnet restore ADMExpedientePersonal.Api.Auth.csproj

# Publicamos la API
RUN dotnet publish ADMExpedientePersonal.Api.Auth.csproj -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ADMExpedientePersonal.Api.Auth.dll"]
