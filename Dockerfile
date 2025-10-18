# Etapa 1: Construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo .csproj y restaura dependencias
COPY CRMVentasAPI/CRMVentasAPI.csproj CRMVentasAPI/
RUN dotnet restore CRMVentasAPI/CRMVentasAPI.csproj

# Copia el resto del código
COPY . .

# Publica el proyecto en modo Release
RUN dotnet publish CRMVentasAPI/CRMVentasAPI.csproj -c Release -o /app/out

# Etapa 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "CRMVentasAPI.dll"]

