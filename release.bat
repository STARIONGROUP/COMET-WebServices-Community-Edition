@echo off

IF %1.==. GOTO VersionError
set version=%1

set releasefolder=.\Release
set buildfolder=.\CDP4WebServer\bin\Release\net472
set buildfolderIIS=.\CDP4WebServer.IIS\bin\app.publish
set logs=logs
set filename=COMETWebServices-%version%.zip
set filenameIIS=COMETWebServices-IIS-%version%.zip

GOTO Setup

:VersionError
ECHO.
ECHO ERROR: No version was specified
ECHO.

GOTO End

:Setup
ECHO Releasing Version %version%

set dry=false

IF %2.==. GOTO Begin
IF %2==dry GOTO Dry

:Dry

ECHO.
ECHO Performing dry run...
ECHO.

set dry=true

:Begin

ECHO.
ECHO Creating release folders
ECHO.

if not exist %releasefolder% mkdir %releasefolder%
if exist %releasefolder%\%version% rmdir %releasefolder%\%version% /S /Q
if not exist %releasefolder%\%version% mkdir %releasefolder%\%version%

ECHO.
ECHO Cleaning up...
ECHO.

call dotnet build CDP4-Server-NO-IIS.sln -target:Clean -p:Configuration=Release

ECHO.
ECHO Removing logs if existing...
ECHO.

if exist %buildfolder%\%logs% rmdir %buildfolder%\%logs% /S /Q

ECHO.
ECHO Building solution....
ECHO.

call dotnet build CDP4-Server-NO-IIS.sln -property:Configuration=Release -restore


ECHO Error Level %errorlevel%

IF %errorlevel%==1 GOTO BuildError

ECHO.
ECHO Cleaning local Images...
ECHO.

REM cleanup previous if exists
docker rmi cdp4-services-community-edition:%version%
docker rmi rheagroup/cdp4-services-community-edition:%version%

REM remove local latest if exists
docker rmi  cdp4-services-community-edition:latest
docker rmi  rheagroup/cdp4-services-community-edition:latest

ECHO.
ECHO Building images...
ECHO.

REM build and tag
docker build -t cdp4-services-community-edition:%version% -t cdp4-services-community-edition:latest .
docker tag cdp4-services-community-edition:%version% rheagroup/cdp4-services-community-edition:%version%
docker tag cdp4-services-community-edition:latest rheagroup/cdp4-services-community-edition:latest

ECHO.
ECHO Building IIS version....
ECHO.

call MSBuild.exe .\CDP4WebServer.IIS\CDP4WebServer.IIS.csproj /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /p:Configuration=Release /p:VisualStudioVersion=14.0

ECHO.
ECHO Building Release Archive
ECHO.

call powershell -Command "& {Compress-Archive -Path %buildfolder%\* -DestinationPath %releasefolder%\%version%\%filename% -Force;}"
call powershell -Command "& {Compress-Archive -Path %buildfolderIIS%\* -DestinationPath %releasefolder%\%version%\%filenameIIS% -Force;}"

ECHO.
ECHO Generating Verification Hashes
ECHO.

call powershell -Command "& {Get-ChildItem -Path %releasefolder%\%version% -Recurse -Filter *.zip | Get-FileHash -Algorithm MD5 | Format-List | Out-File %releasefolder%\%version%\verification_hashes_%version%.txt}"
call powershell -Command "& {Get-ChildItem -Path %releasefolder%\%version% -Recurse -Filter *.zip | Get-FileHash | Format-List | Out-File %releasefolder%\%version%\verification_hashes_%version%.txt -Append}"


IF %dry% equ true GOTO End

ECHO.
ECHO Pushing...
ECHO.

REM push
docker push rheagroup/cdp4-services-community-edition:%version%
docker push rheagroup/cdp4-services-community-edition:latest

:End

ECHO.
ECHO Release %version% Completed
ECHO.

EXIT /B 0

:BuildError

ECHO.
ECHO Release %version% %platform% Failed
ECHO.

EXIT /B 1