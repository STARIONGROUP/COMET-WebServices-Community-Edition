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
ECHO Cleaning up...
ECHO.

REM cleanup previous if exists
docker rmi comet-webservices-community-edition:%version%
docker rmi docker.pkg.github.com/stariongroup/cdp4-webservices-community-edition/comet-webservices-community-edition:%version%

REM remove local latest if exists
docker rmi  comet-webservices-community-edition:latest-rc
docker rmi  docker.pkg.github.com/stariongroup/cdp4-webservices-community-edition/comet-webservices-community-edition:latest-rc

ECHO.
ECHO Building images...
ECHO.

REM build and tag
docker build -t comet-webservices-community-edition:%version% -t comet-webservices-community-edition:latest-rc .
docker tag comet-webservices-community-edition:%version% docker.pkg.github.com/stariongroup/cdp4-webservices-community-edition/comet-webservices-community-edition:%version%
docker tag comet-webservices-community-edition:latest-rc docker.pkg.github.com/stariongroup/cdp4-webservices-community-edition/comet-webservices-community-edition:latest-rc

IF %dry% equ true GOTO End

ECHO.
ECHO Pushing...
ECHO.

REM push
docker push docker.pkg.github.com/stariongroup/cdp4-webservices-community-edition/comet-webservices-community-edition:%version%
docker push docker.pkg.github.com/stariongroup/cdp4-webservices-community-edition/comet-webservices-community-edition:latest-rc

:End

ECHO.
ECHO Release Candidate %version% Completed
ECHO.