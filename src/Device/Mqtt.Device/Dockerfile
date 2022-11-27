FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Device/Mqtt.Device/Mqtt.Device.csproj", "Device/Mqtt.Device/"]
RUN dotnet restore "Device/Mqtt.Device/Mqtt.Device.csproj"
COPY . .
WORKDIR "/src/Device/Mqtt.Device"
RUN dotnet build "Mqtt.Device.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mqtt.Device.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mqtt.Device.dll"]
