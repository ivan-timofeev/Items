FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Items/Items.csproj", "Items/"]
RUN dotnet restore "Items/Items.csproj"
COPY . .
WORKDIR "/src/Items"
RUN dotnet build "Items.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Items.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Items-Backend.dll"]




