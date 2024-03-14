FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY CDP4Authentication CDP4Authentication
COPY CDP4DatabaseAuthentication CDP4DatabaseAuthentication
COPY CDP4WspDatabaseAuthentication CDP4WspDatabaseAuthentication
COPY CDP4Orm CDP4Orm
COPY VersionFileCreator VersionFileCreator
COPY CometServer CometServer

RUN dotnet build CDP4DatabaseAuthentication -c Release
RUN dotnet build CDP4WspDatabaseAuthentication -c Release
RUN dotnet publish -r linux-x64 CometServer -c Release -o /app/CometServer/bin/Release/publish 

FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.19
WORKDIR /app
RUN mkdir /app/logs
RUN mkdir /app/storage
RUN mkdir /app/tempstorage
RUN mkdir /app/upload

RUN mkdir /app/Authentication/
RUN mkdir /app/Authentication/CDP4Database
RUN mkdir /app/Authentication/CDP4WspDatabase

COPY --from=build-env /app/CometServer/bin/Release/publish .
RUN rm /app/appsettings.Development.json
RUN mv /app/appsettings.Production.json /app/appsettings.json

# COPY CDP4DatabaseAuthentication plugin
COPY --from=build-env /app/CDP4DatabaseAuthentication/bin/Release/CDP4DatabaseAuthentication.dll /app/Authentication/CDP4Database/CDP4DatabaseAuthentication.dll
COPY --from=build-env /app/CDP4DatabaseAuthentication/bin/Release/config.json /app/Authentication/CDP4Database/config.json

# COPY CDP4WspDatabaseAuthentication plugin
COPY --from=build-env /app/CDP4WspDatabaseAuthentication/bin/Release/CDP4WspDatabaseAuthentication.dll /app/Authentication/CDP4WspDatabase/CDP4WspDatabaseAuthentication.dll
COPY --from=build-env /app/CDP4WspDatabaseAuthentication/bin/Release/config.json /app/Authentication/CDP4WspDatabase/config.json

# set to use the non-root USER here
RUN chown -R $APP_UID /app
USER $APP_UID 
CMD ["dotnet", "CometServer.dll"]