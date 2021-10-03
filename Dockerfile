FROM mcr.microsoft.com/dotnet/sdk:5.0-bullseye-slim AS build-env
WORKDIR /app
COPY CDP4Authentication CDP4Authentication
COPY CDP4DatabaseAuthentication CDP4DatabaseAuthentication
COPY CDP4WspDatabaseAuthentication CDP4WspDatabaseAuthentication
COPY CDP4Orm CDP4Orm
COPY CometServer CometServer

RUN dotnet build CDP4DatabaseAuthentication -c Release
RUN dotnet build CDP4WspDatabaseAuthentication -c Release
RUN dotnet publish CometServer -c Release

FROM mcr.microsoft.com/dotnet/aspnet:5.0-bullseye-slim
WORKDIR /app
RUN mkdir /app/logs
RUN mkdir /app/storage
RUN mkdir /app/upload
COPY --from=build-env /app/CometServer/bin/Release/publish .

CMD ["dotnet", "CometServer.dll"]