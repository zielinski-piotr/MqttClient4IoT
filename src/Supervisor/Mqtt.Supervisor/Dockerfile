FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Supervisor/Mqtt.Supervisor/Mqtt.Supervisor.csproj", "Supervisor/Mqtt.Supervisor/"]
RUN dotnet restore "Supervisor/Mqtt.Supervisor/Mqtt.Supervisor.csproj"
COPY . .
WORKDIR "/src/Supervisor/Mqtt.Supervisor"
RUN dotnet build "Mqtt.Supervisor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mqtt.Supervisor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mqtt.Supervisor.dll"]
