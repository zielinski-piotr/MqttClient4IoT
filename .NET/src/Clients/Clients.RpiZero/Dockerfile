FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Clients/Clients.RpiZero/Clients.RpiZero.csproj", "Clients/Clients.RpiZero/"]
RUN dotnet restore "Clients/Clients.RpiZero/Clients.RpiZero.csproj"
COPY . .
WORKDIR "Clients/Clients.RpiZero"
RUN dotnet build "Clients.RpiZero.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clients.RpiZero.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clients.RpiZero.dll"]
