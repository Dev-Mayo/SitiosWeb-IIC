# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos toda la solución
COPY . .

# Restauramos dependencias
RUN dotnet restore ./Services/ADMExpedientePersonal.Api.Empleados/ADMExpedientePersonal.Api.Empleados.csproj

# Publicamos la API
RUN dotnet publish ./Services/ADMExpedientePersonal.Api.Empleados/ADMExpedientePersonal.Api.Empleados.csproj -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ADMExpedientePersonal.Api.Auth.dll"]
