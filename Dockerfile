FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY CDP4Authentication CDP4Authentication
COPY CDP4DatabaseAuthentication CDP4DatabaseAuthentication
COPY CDP4WspDatabaseAuthentication CDP4WspDatabaseAuthentication
COPY CDP4Orm CDP4Orm
COPY CometServer CometServer

RUN dotnet build CDP4DatabaseAuthentication -c Release
RUN dotnet build CDP4WspDatabaseAuthentication -c Release
RUN dotnet publish CometServer -c Release

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
RUN mkdir /app/logs
RUN mkdir /app/storage
RUN mkdir /app/upload

RUN mkdir /app/Authentication/
RUN mkdir /app/Authentication/CDP4Database
RUN mkdir /app/Authentication/CDP4WspDatabase

COPY --from=build-env /app/CometServer/bin/Release/publish .

# COPY CDP4DatabaseAuthentication plugin
COPY --from=build-env /app/CDP4DatabaseAuthentication/bin/Release/CDP4DatabaseAuthentication.dll /app/Authentication/CDP4Database/CDP4DatabaseAuthentication.dll
COPY --from=build-env /app/CDP4DatabaseAuthentication/bin/Release/config.json /app/Authentication/CDP4Database/config.json

# COPY CDP4WspDatabaseAuthentication plugin
COPY --from=build-env /app/CDP4WspDatabaseAuthentication/bin/Release/CDP4WspDatabaseAuthentication.dll /app/Authentication/CDP4WspDatabase/CDP4WspDatabaseAuthentication.dll
COPY --from=build-env /app/CDP4WspDatabaseAuthentication/bin/Release/config.json /app/Authentication/CDP4WspDatabase/config.json

CMD ["dotnet", "CometServer.dll"]