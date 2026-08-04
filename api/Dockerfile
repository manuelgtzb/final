FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY LandingFinal.Api.csproj ./
RUN dotnet restore LandingFinal.Api.csproj

COPY . ./
RUN dotnet publish LandingFinal.Api.csproj --configuration Release --no-restore --output /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0
ENV PORT=10000

EXPOSE 10000

COPY --from=build /app/publish ./

USER $APP_UID
ENTRYPOINT ["dotnet", "LandingFinal.Api.dll"]
