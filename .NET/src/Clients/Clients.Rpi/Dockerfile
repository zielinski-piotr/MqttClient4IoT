FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Clients/Clients.Rpi/Clients.Rpi.csproj", "Clients/Clients.Rpi/"]
RUN dotnet restore "Clients/Clients.Rpi/Clients.Rpi.csproj"
COPY . .
WORKDIR "Clients/Clients.Rpi"
RUN dotnet build "Clients.Rpi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clients.Rpi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clients.Rpi.dll"]
