@ECHO OFF
mode con: cols=120 lines=40
setLocal EnableDelayedExpansion


set currentVersion=1.0
set currentBuild=0
set currentRevision=0
set configuration=Release
set nsisScriptName=Homidom.Setup_Full_x86_x64


set makensis="tools\nsis-2.46\makensis.exe"



ECHO Numero de Build (%currentVersion%.xxx.0) :
set /p currentBuild=Numero de release [%currentBuild%]: 

ECHO Numero de Revision (%currentVersion%.%currentBuild%.xxxx) :
set /p currentRevision=Numero de release [%currentRevision%]: 

CHOICE /c ON /M "Voulez-vous creer un package pour la version %currentVersion%.%currentBuild%.%currentRevision% ?"
IF ERRORLEVEL 2 GOTO Cleaning


:Build

MKDIR Packages

:Package
tools\sed -e "s/#PRODUCT_REVISION#/%currentRevision%/g;s/#PRODUCT_BUILD#/%currentBuild%/g;s/#PRODUCT_VERSION#/%currentVersion%/g;" %nsisScriptName%.nsi > %nsisScriptName%.%currentRevision%.nsi

%makensis% %nsisScriptName%.%currentRevision%.nsi


:Cleaning
IF EXIST %nsisScriptName%.%currentRevision%.nsi DEL %nsisScriptName%.%currentRevision%.nsi
