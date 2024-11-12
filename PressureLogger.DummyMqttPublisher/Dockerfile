FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PressureLogger.DummyMqttPublisher/PressureLogger.DummyMqttPublisher.csproj", "PressureLogger.DummyMqttPublisher/"]
RUN dotnet restore "PressureLogger.DummyMqttPublisher/PressureLogger.DummyMqttPublisher.csproj"
COPY . .
WORKDIR "/src/PressureLogger.DummyMqttPublisher"
RUN dotnet build "PressureLogger.DummyMqttPublisher.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PressureLogger.DummyMqttPublisher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PressureLogger.DummyMqttPublisher.dll"]
