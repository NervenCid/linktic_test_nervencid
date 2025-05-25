# Imagen base de ASP.NET Core para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5071

# Imagen base de SDK para compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo de proyecto y restaura dependencias
COPY ProductsService.csproj .
RUN dotnet restore "ProductsService.csproj"

# Copia el resto del código fuente y publica la aplicación
COPY . .
RUN dotnet publish "ProductsService.csproj" -c Release -o /app/publish

# Imagen final para ejecución
FROM base AS final
WORKDIR /app

# Copia los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Comando de entrada para ejecutar el servicio
ENTRYPOINT ["dotnet", "ProductsService.dll"]