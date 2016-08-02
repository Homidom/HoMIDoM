@ECHO OFF
mode con: cols=120 lines=40
setLocal EnableDelayedExpansion


set currentVersion=1.3
set currentBuild=0
set currentRevision=0
set currentRelease=STABLE
set nsisScriptName=Homidom.Setup_Full_x86_x64


rem set makensis="tools\nsis-2.46\makensis.exe"
set makensis="tools\nsis-2.51\makensis.exe"
set /p currentVersion=Numero de Version (xx.xx.0.0) [%currentVersion%]:
set /p currentBuild=Numero de Build (%currentVersion%.xxx.0) [%currentBuild%]: 
set /p currentRevision=Numero de Revision (%currentVersion%.%currentBuild%.xxxx) [%currentRevision%]: 
set /p currentRelease=Type de Release (STABLE / RELEASE) [%currentRelease%]:
CHOICE /c ON /M "Voulez-vous creer un package pour la version %currentVersion%.%currentBuild%.%currentRevision% ?"
IF ERRORLEVEL 2 GOTO Cleaning


:Build

MKDIR Packages

:Package
tools\sed -e "s/#PRODUCT_REVISION#/%currentRevision%/g;s/#PRODUCT_BUILD#/%currentBuild%/g;s/#PRODUCT_VERSION#/%currentVersion%/g;s/#PRODUCT_RELEASE#/%currentRelease%/g;" %nsisScriptName%.nsi > %nsisScriptName%.%currentRevision%.nsi

%makensis% %nsisScriptName%.%currentRevision%.nsi


:Cleaning
IF EXIST %nsisScriptName%.%currentRevision%.nsi DEL %nsisScriptName%.%currentRevision%.nsi
