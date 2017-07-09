@ECHO OFF
mode con: cols=120 lines=40
setLocal EnableDelayedExpansion

REM Creating a Newline variable (the two blank lines are required!)
set NLM=^


set NL=^^^%NLM%%NLM%^%NLM%%NLM%

set currentVersion=1.3
set currentBuild=0
set currentRevision=0
set currentRelease=STABLE
set nsisScriptName=Homiwpf.Setup_Full_x86_x64


set makensis="tools\nsis-2.46\makensis.exe"

ECHO ----------------------------------------------------
ECHO -                                                  -
ECHO -   Creation d'un package d'installation HoMIDoM   -
ECHO -                                                  -
ECHO ----------------------------------------------------
ECHO -

set /p currentVersion=-   Numero de Version (xx.xx.0.0) [%currentVersion%]:
set /p currentBuild=-   Numero de Build (%currentVersion%.xxx.0) [%currentBuild%]: 
set /p currentRevision=-   Numero de Revision (%currentVersion%.%currentBuild%.xxxx) [%currentRevision%]: 
set /p currentRelease=-   Type de Release (STABLE / RELEASE) [%currentRelease%]:
ECHO -
CHOICE /c ON /M "-   Voulez-vous creer un package pour la version %currentVersion%.%currentBuild%.%currentRevision% ?"
IF ERRORLEVEL 2 GOTO Cleaning

:Build

ECHO -%NL%-   Creation du package en cours...
MKDIR Packages 2>NUL

:Package
tools\sed -e "s/#PRODUCT_REVISION#/%currentRevision%/g;s/#PRODUCT_BUILD#/%currentBuild%/g;s/#PRODUCT_VERSION#/%currentVersion%/g;s/#PRODUCT_RELEASE#/%currentRelease%/g;" %nsisScriptName%.nsi > %nsisScriptName%.%currentRevision%.nsi

%makensis% %nsisScriptName%.%currentRevision%.nsi >Packages\build.log
set ERREUR=%ERRORLEVEL%
IF %ERREUR% == 0 ECHO -%NL%-   La compilation a reussi, le package a ete cree dans le repertoire Packages%NL%-   Verifiez le fichier log (eventuel warning) dans : Packages\build.log%NL%-
IF NOT %ERREUR% == 0 ECHO -%NL%- La compilation a echoue, voir detail dans Packages\build.log%NL%-


:Cleaning
IF EXIST %nsisScriptName%.%currentRevision%.nsi DEL %nsisScriptName%.%currentRevision%.nsi

Pause