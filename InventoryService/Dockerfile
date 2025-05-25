FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5117

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY InventoryService.csproj .
RUN dotnet restore "InventoryService.csproj"
COPY . .
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]