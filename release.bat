@echo off

IF %1.==. GOTO VersionError
set version=%1

GOTO Setup

:VersionError
ECHO.
ECHO ERROR: No version was specified
ECHO.

GOTO End

:Setup

ECHO Releasing CDP4-COMET Webservices Version %version%

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
ECHO Cleaning up...
ECHO.

REM cleanup previous if exists
docker rmi comet-webservices-community-edition:%version%
docker rmi stariongroup/comet-webservices-community-edition:%version%

REM remove local latest if exists
docker rmi  comet-webservices-community-edition:latest
docker rmi  stariongroup/comet-webservices-community-edition:latest

ECHO.
ECHO Building images...
ECHO.

REM build and tag
docker build -t comet-webservices-community-edition:%version% -t comet-webservices-community-edition:latest .
docker tag comet-webservices-community-edition:%version% stariongroup/comet-webservices-community-edition:%version%
docker tag comet-webservices-community-edition:latest stariongroup/comet-webservices-community-edition:latest

IF %dry% equ true GOTO End

ECHO.
ECHO Pushing...
ECHO.

REM push
docker push stariongroup/comet-webservices-community-edition:%version%
docker push stariongroup/comet-webservices-community-edition:latest

:End

ECHO.
ECHO Release %version% Completed
ECHO.