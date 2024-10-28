FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build


WORKDIR /src

COPY PressureLogger.UI.csproj PressureLogger.UI/

RUN dotnet restore PressureLogger.UI/PressureLogger.UI.csproj

COPY . .
RUN dotnet build PressureLogger.UI.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish PressureLogger.UI.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .

#Certs
COPY Certificates/cert.pem /etc/ssl/certs/cert.pem
COPY Certificates/key.pem /etc/ssl/private/key.pem

COPY nginx.conf /etc/nginx/nginx.conf


EXPOSE 443
EXPOSE 80