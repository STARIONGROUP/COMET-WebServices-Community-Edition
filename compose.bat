@echo off

set DB_POSTGRESPASSWORD=pass
set DB_HOSTPORT=5432
set WEBSERVICES_HOSTPORT=5000

IF %1.==. GOTO Build
IF %1==build GOTO Build
IF %1==up GOTO Up
IF %1==down GOTO Down
IF %1==strt GOTO Strt
IF %1==stp GOTO Stp
IF %1==reboot GOTO Reboot
IF %1==rebuild GOTO Rebuild

GOTO End

:Build
rem Need to build the application in Release before making the image
START /B MSBuild.exe CDP4-Server.sln -property:Configuration=Release -restore
START /B docker-compose up --build
GOTO End

:Strt
START /B docker-compose start
GOTO End

:Stp
START /B docker-compose stop
GOTO End

:Up
START /B docker-compose up -d
GOTO End

:Down
START /B docker-compose down --remove-orphans
GOTO End

:Reboot
START /B docker-compose down
START /B docker-compose up -d
GOTO End

:Rebuild
START /B docker-compose down
rem Need to build the application in Release before making the image
START /B MSBuild.exe CDP4-Server.sln -property:Configuration=Release -restore
START /B docker-compose up --build -d
GOTO End

:End