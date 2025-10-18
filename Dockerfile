# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia todo el contenido del proyecto
COPY . .

# Restaura las dependencias
RUN dotnet restore

# Publica el proyecto en modo Release en la carpeta /out
RUN dotnet publish -c Release -o out

# Etapa 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia el resultado de la compilación
COPY --from=build /app/out .

# Expone el puerto (Render usa 10000 internamente)
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

# Comando de inicio
ENTRYPOINT ["dotnet", "CRMVentasAPI.dll"]

