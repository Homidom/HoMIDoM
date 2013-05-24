@ECHO OFF
hitb.exe -cis
IF %ERRORLEVEL% == 0 ECHO SVC NOT INSTALLED
IF %ERRORLEVEL% == 1 ECHO SVC INSTALLED

hitb.exe -crs
IF %ERRORLEVEL% == 0 ECHO SVC NOT RUNNING
IF %ERRORLEVEL% == 1 ECHO SVC RUNNING

hitb.exe -cra
IF %ERRORLEVEL% == 0 ECHO APP NOT RUNNING
IF %ERRORLEVEL% == 1 ECHO APP RUNNING

hitb.exe -dpi
IF %ERRORLEVEL% == 0 ECHO PREVIOUS INSTALL NOT DETECTED
IF %ERRORLEVEL% == 1 ECHO PREVIOUS INSTALL DETECTED

PAUSE