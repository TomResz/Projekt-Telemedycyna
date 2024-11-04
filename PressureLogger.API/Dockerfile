FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV TZ=Europe/Warsaw
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
USER app
WORKDIR /app
EXPOSE 6000
EXPOSE 6001


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PressureLogger.API/PressureLogger.API.csproj", "PressureLogger.API/"]
COPY ["PressureLogger.Infrastructure/PressureLogger.Infrastructure.csproj", "PressureLogger.Infrastructure/"]
RUN dotnet restore "./PressureLogger.API/PressureLogger.API.csproj"
COPY . .
WORKDIR "/src/PressureLogger.API"
RUN dotnet build "./PressureLogger.API.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PressureLogger.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PressureLogger.API.dll"]