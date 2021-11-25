FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

ENV ASPNETCORE_ENVIRONMENT=Production

COPY . ./
RUN dotnet publish SpotifyGateway.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS http://*:5000

ENTRYPOINT ["dotnet", "SpotifyGateway.dll"]