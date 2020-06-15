@echo off

set DB_POSTGRESPASSWORD=pass
set DB_HOSTPORT=5432
set DB_TESTHOSTPORT=5432
set WEBSERVICES_HOSTPORT=5000

IF %1.==. GOTO Build
IF %1==build GOTO Build
IF %1==up GOTO Up
IF %1==down GOTO Down
IF %1==strt GOTO Strt
IF %1==stp GOTO Stp
IF %1==reboot GOTO Reboot
IF %1==rebuild GOTO Rebuild
IF %1==dev GOTO Dev
IF %1==devbg GOTO DevBg
IF %1==devtest GOTO DevTest
IF %1==devtestbg GOTO DevTestBg
IF %1==devdown GOTO DevDown
IF %1==devtestdown GOTO DevTestDown

GOTO End

:Build
rem Need to build the application in Release before making the image
CALL MSBuild.exe CDP4-Server.sln -property:Configuration=Release -restore
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

:Dev
START /B docker-compose -f docker-compose-dev.yml down --remove-orphans
START /B docker-compose -f docker-compose-dev.yml up --build
GOTO End

:DevBg
START /B docker-compose -f docker-compose-dev.yml down --remove-orphans
START /B docker-compose -f docker-compose-dev.yml up --build -d
GOTO End

:DevTest
START /B docker-compose -f docker-compose-dev-test.yml down --remove-orphans
START /B docker-compose -f docker-compose-dev-test.yml up --build
GOTO End

:DevTestBg
START /B docker-compose -f docker-compose-dev-test.yml down --remove-orphans
START /B docker-compose -f docker-compose-dev-test.yml up --build -d
GOTO End

:DevDown
START /B docker-compose -f docker-compose-dev.yml down --remove-orphans
GOTO End

:DevTestDown
START /B docker-compose -f docker-compose-dev-test.yml down --remove-orphans
GOTO End


:End