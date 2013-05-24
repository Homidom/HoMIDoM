; **********************************************************************
; Homidom Install & Update script
; **********************************************************************

; --- définition des constantes de base
!define PRODUCT_NAME "HoMIDoM"
!define PRODUCT_VERSION "#PRODUCT_VERSION#"
!define PRODUCT_BUILD "#PRODUCT_BUILD#"
!define PRODUCT_REVISION "#PRODUCT_REVISION#"
!define PRODUCT_RELEASE "RELEASE"
!define PRODUCT_PUBLISHER "HoMIDoM"
!define PRODUCT_WEB_SITE "http://www.homidom.fr"
!define PRODUCT_DIR_REGKEY "Software\${PRODUCT_NAME}"
!define PRODUCT_UNINSTALL_REGKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"

; --- Version du Framework .NET requis et URL de téléchargement
!define DOT_MAJOR "4"
!define DOT_MINOR "0"
!define URL_DOTNET "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe"
; --- inclusion du code de détection et de téléchargement du Framework .NET
!include "includes\CheckDotNetInstalled.nsh"

; --- Dossier racine contenant les sources (relatif à l'emplacement du script NSIS)
!define ROOT_DIR ".."
; --- Dossier de publication => dossier contenant les binaires
!define PUBLISH_DIR "${ROOT_DIR}\${PRODUCT_RELEASE}"
; --- Dossier de destination du package généré (relatif à l'emplacement du script NSIS)
!define PACKAGES_DIR "Packages"

; --- Définition des constantes permettant de mettre à jour les informations de version (VI) de l'executable généré.
VIProductVersion "${PRODUCT_VERSION}.${PRODUCT_BUILD}.${PRODUCT_REVISION}"
VIAddVersionKey "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey "Comments" "${PRODUCT_WEB_SITE}"
VIAddVersionKey "CompanyName" "${PRODUCT_NAME}"
VIAddVersionKey "LegalCopyright" "Copyright ${PRODUCT_NAME}"
VIAddVersionKey "FileDescription" "${PRODUCT_NAME}"
VIAddVersionKey "FileVersion" "${PRODUCT_VERSION}.${PRODUCT_BUILD}.${PRODUCT_REVISION}"

; MUI 1.67 compatible ------
!include "MUI2.nsh"
!include "Sections.nsh"
!include "TextFunc.nsh"
!include "LogicLib.nsh"
!include "includes\x64.nsh"
!include "nsDialogs.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_COMPONENTSPAGE_NODESC
!define MUI_ICON "${ROOT_DIR}\Images\Logo\icone.ico"
!define MUI_UNICON "${ROOT_DIR}\Images\Logo\icone.ico"

!define TEMP1 $R0 ;Temp variable

; Welcome page
;!insertmacro MUI_PAGE_WELCOME
; License page
!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "includes\gpl-3.0.txt"
; Components page
!insertmacro MUI_PAGE_COMPONENTS
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Custom option page
Page custom nsDialogsPage nsDialogsPageLeave
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!insertmacro MUI_PAGE_FINISH


;!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!include "includes\localization.fr.nsh"
;!include "includes\localization.en.nsh"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}.${PRODUCT_BUILD} (${PRODUCT_REVISION})"
OutFile "${PACKAGES_DIR}\${PRODUCT_NAME}.Setup.${PRODUCT_VERSION}.${PRODUCT_BUILD}.${PRODUCT_REVISION}_Full_x86_x64.exe"
InstallDir $PROGRAMFILES\HoMIDoM
BrandingText "HoMIDoM Installer v1.0"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" "InstallDir"
ShowInstDetails show
ShowUnInstDetails show

; --- définition des variables de la boite de dialogue personalisé 
Var Dialog
var chkInstallAsService_Handle
Var optInstallAsService
var chkStartService_Handle
Var optStartService
Var chkCreateStartMenuShortcuts_Handle
Var optCreateStartMenuShortcuts

; Request application privileges for Windows Vista
RequestExecutionLevel admin

; --- définition des variables personalisées
Var IsHomidomInstalled ;indique si HoMIDom est déja installé
Var IsHomidomInstalledAsService ;indique si HoMIDoMService est installé en tant que service windows
Var IsHomidomServiceRunning ;indique si le service HoMIDoMService est démarré
Var IsHomidomAppRunning ;indique si une app Homidom est en cours d'execution (Admin,WPF, )
Var HomidomInstalledVersion ;Numéro de version Homidom.dll
Var IsPreviousInstallIsDeprecated ;Indique si la précedente installation a été faite avec l'ancien installeur (msi)

; **********************************************************************
; Initialisation - code exécuté avant l'affichage de la 1iere fenêtre
; **********************************************************************
Function .onInit

  ; -- Création du dossier temporaires nécessaires à l'installeur (%TEMP%\ns*****)
  InitPluginsDir

  ; --- déploiement des fichiers nécessaires à l'installation
  File /oname=$PLUGINSDIR\nssm.exe "tools\nssm-2.16\win32\nssm.exe"
  File /oname=$PLUGINSDIR\hitb.exe "tools\hitb-1.0\win32\hitb.exe"
    
  # --- affichage du Logo "splah"
  File /oname=$PLUGINSDIR\splash.bmp "..\Images\logo\logo.bmp"  
  advsplash::show 2000 600 400 -1 $PLUGINSDIR\splash
  Pop $0

  ; --- sélection de la langue
  !insertmacro MUI_LANGDLL_DISPLAY

  ; --- Vérification de la version du Framework .NET
  Call CheckDotNetInstalled
  
  ; --- détection de l'OS (32bits/64bits) & pré-configuration du dossier de destination
  StrCpy $instdir "$programfiles32\${PRODUCT_NAME}"
  ${If} ${RunningX64}
        StrCpy $instdir "$programfiles64\${PRODUCT_NAME}"
  ${EndIf}
  
  ; --- Detection d'un installation existante
  StrCpy $IsHomidomInstalled "0"
  StrCpy $IsPreviousInstallIsDeprecated "0"

  ; --- Détection basée sur la présence de la DLL principale dans le dossier d'install par défaut -ou- spécifié en BdR si déja installé
  IfFileExists "$INSTDIR\HoMIDom.dll" 0 homidomIsNotInstalled

  ; --- Installation existante détectée !
  StrCpy $IsHomidomInstalled "1"
  ; --- récupération de la version installée => $HomidomInstalledVersion
  ${GetFileVersion} "$INSTDIR\HoMIDom.dll" $HomidomInstalledVersion

  ; --- Détection du type d'installation du service (service windows ou pas) -> 0: Not found, 1: Found
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -cis"' ""
  Pop $IsHomidomInstalledAsService
  StrCpy $optInstallAsService $IsHomidomInstalledAsService

  ; --- Détection de l'installeur utilisé précédement
  ; Impossible de lire cette entrée de registre ... pb de droit ??? => on se base donc sur la présence d'un fichier spécifique à l'ancien installeur :Uninstall.vbs
  ;ReadRegStr $0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{7FC2F25A-2D7C-48B8-88C8-4D1EE59ED19E}" "DisplayVersion"
  ;StrCmp "$0" "" homidomIsNotInstalled 0
  ;IfFileExists "$INSTDIR\Uninstall.vbs" 0 homidomIsNotInstalled
  ;StrCpy $IsPreviousInstallIsDeprecated "1"

  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -dpi"' ""
  Pop $IsPreviousInstallIsDeprecated


homidomIsNotInstalled:
    
FunctionEnd

; **********************************************************************
; Success - code exécuté à la fin de l'installation (en cas de success)
; **********************************************************************
Function .onInstSuccess

  ; Write the installation path into the registry
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "InstallDir" "$INSTDIR"
  
  ; Recuperation de la taille de l'installation
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
   
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "DisplayName" "${PRODUCT_NAME}"
  WriteRegStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "Publisher" "${PRODUCT_NAME}"
  WriteRegStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "DisplayIcon" "$INSTDIR\Images\icone.ico"
  WriteRegStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "Comments" "HoMIDoM, la solution gratuite et multi-technologie de domotique"
  WriteRegStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "DisplayVersion" "${PRODUCT_VERSION}.${PRODUCT_BUILD}.${PRODUCT_REVISION}"
  WriteRegStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "${PRODUCT_UNINSTALL_REGKEY}" "NoModify" 1
  WriteRegDWORD HKLM "${PRODUCT_UNINSTALL_REGKEY}" "NoRepair" 1
  WriteRegExpandStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "HelpLink" "http://www.homidom.com"
  WriteRegDWORD HKLM "${PRODUCT_UNINSTALL_REGKEY}" "EstimatedSize" "$0"
  WriteUninstaller "uninstall.exe"

  ; --- Démarrage du serveur (si nécessaire)
  StrCmp "$optStartService" "0" skipStartService
  StrCmp "$optInstallAsService" "1" skipStartService
    SetOutPath "$INSTDIR"
    Exec '"$INSTDIR\HomidomService.exe"'

skipStartService:

FunctionEnd

; --- Définition des profils (types) d'installation
InstType "Complete"
InstType "Service uniquement"
InstType "Admin uniquement"

Section "" HoMIDoM_STOPSVC

  SectionIn 1 2
  
  Banner::show /NOUNLOAD ""

  ; --- Vérifie si le service Homidom est démarré
  ; 0: Not running or Not found
  ; 1: Found and Running
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -crs"' ""
  Pop $IsHomidomServiceRunning ; return value - process exit code or error or STILL_ACTIVE (0x103).

  ; --- Vérifie si des app Homidom sont en cours d'execution
  ; 0: Not running or Not found apps
  ; 1: Found and Running
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -cra"' ""
  Pop $IsHomidomAppRunning ; return value - process exit code or error or STILL_ACTIVE (0x103).

  Banner::destroy

  StrCmp "$IsHomidomAppRunning" "1" 0 noHomidomAppRunning
    MessageBox MB_ICONEXCLAMATION|MB_YESNO "Une application ${PRODUCT_NAME} est en cours d'execution. Voulez-vous l'arreter ?" IDYES killHomidomNow IDNO 0
    Abort

noHomidomAppRunning:
  StrCmp "$IsHomidomServiceRunning" "1" 0 noHomidomSvcRunning
    MessageBox MB_ICONEXCLAMATION|MB_YESNO "Le serveur ${PRODUCT_NAME} est en cours d'execution. Voulez-vous l'arreter  ?" IDYES killHomidomNow IDNO 0
    Abort

killHomidomNow:
  call KillAllHomidomServices

noHomidomSvcRunning:
  ; si homidom server installé en tant que service et si l'utilisateur a désactiover cette option => désinstallation du service.
  StrCmp "$IsHomidomInstalledAsService" "1" 0 checkIfPreviousInstallIsDeprecated
  StrCmp "$optInstallAsService" "0" 0 checkIfPreviousInstallIsDeprecated
    call UnInstallHomidomService

checkIfPreviousInstallIsDeprecated:
  StrCmp "$IsPreviousInstallIsDeprecated" "1" 0 extStopSvcSection

    MessageBox MB_ICONEXCLAMATION|MB_YESNO "Une ancienne installation a été détectée. Voulez-vous la désinstaller maintenant ? $\n(votre configuration sera sauvegardée et rechargée automatiquement)"  IDYES backupPreviousFiles IDNO 0
    Abort

backupPreviousFiles:
  ; --- Sauvegarde des fichiers de l'ancienne installation
  Banner::show /NOUNLOAD "Sauvegarde..."
  ClearErrors
  CopyFiles $INSTDIR\* "$DOCUMENTS\HoMIDoM.Backup"
  Delete "$DOCUMENTS\HoMIDoM.Backup\Uninstall.vbs"
  Delete "$DOCUMENTS\HoMIDoM.Backup\Install.vbs"
  Banner::destroy
  
  IFErrors 0 uninstallPreviousVersion
    MessageBox MB_ICONSTOP "Echec de la sauvegarde des fichiers de l'ancienne installation !"
    Abort
    
uninstallPreviousVersion:
  ; --- Désinstallation
  ClearErrors
  ;ExecWait "msiexec /x {7FC2F25A-2D7C-48B8-88C8-4D1EE59ED19E} /passive"
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -upi"' ""
  Pop $0
  StrCmp "$0" "0" 0 restorePreviousFile
    MessageBox MB_ICONSTOP "Echec de la désintallation de l'ancienne installation !"
    Abort

restorePreviousFile:
  Banner::show /NOUNLOAD "Restauration..."
  ClearErrors
  CopyFiles "$DOCUMENTS\HoMIDoM.Backup\*" $INSTDIR
  Banner::destroy
  
extStopSvcSection:

SectionEnd

Section "" HoMIDoM_FRAMEWORK

  ; Fichiers de base : requis par l'ensemble des applications de la solution
  ; Section obligatoire.
  SectionIn RO

  CreateDirectory "$INSTDIR"
  CreateDirectory "$INSTDIR\Bdd"
  CreateDirectory "$INSTDIR\Config"
  CreateDirectory "$INSTDIR\Drivers"
  CreateDirectory "$INSTDIR\Fichiers"
  CreateDirectory "$INSTDIR\Images"
  CreateDirectory "$INSTDIR\Images\Users"
  CreateDirectory "$INSTDIR\Logs"
  CreateDirectory "$INSTDIR\Templates"
  
  SetOutPath "$INSTDIR"
  File "tools\hitb-1.0\win32\hitb.exe"
  File "..\RELEASE\HoMIDom.DLL"
  File "..\RELEASE\NCrontab.dll"
    
  ${If} ${RunningX64}
    File  /x "*.xml" /x "*.pdb" /x "MjpegProcessor.*" "..\Dll_externes\Homidom-64bits\*"
  ${Else}
    File  /x "*.xml" /x "*.pdb" /x "MjpegProcessor.*" "..\Dll_externes\Homidom-32bits\*"
  ${EndIf}

  SetOutPath "$INSTDIR\Templates"
  File /r "..\RELEASE\Templates\*"

SectionEnd

SectionGroup "HoMIDoM Service" HoMIDoM_SERVICE_GRP

   Section "Service" HoMIDoM_SERVICE

    SectionIn 1 2

    SetOverwrite on
    SetOutPath "$INSTDIR"
    File "..\RELEASE\HoMIDomService.exe"
    File "..\RELEASE\HoMIDomWebAPI.dll"

    SetOverwrite off
    SetOutPath "$INSTDIR\Bdd"
    File "..\RELEASE\Bdd\homidom.db"
    File "..\RELEASE\Bdd\medias.db"

    SetOverwrite off
    SetOutPath "$INSTDIR\Config"
    File "..\RELEASE\Config\homidom.xml"

    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_System.dll"
    File "..\RELEASE\Drivers\Driver_Virtuel.dll"
    File "..\RELEASE\Drivers\Driver_Weather.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\Interop.WUApiLib.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\Interop.WUApiLib.dll"
    ${EndIf}

  SectionEnd
  
  SectionGroup "Drivers" DRIVERS_GRP
    !include "includes\Driver_*.nsh"
  SectionGroupEnd
  
SectionGroupEnd


Section "HoMIDoM Admin" HoMIDoM_ADMIN
  
  SectionIn 1 3

  SetOverwrite on
  SetOutPath "$INSTDIR"
  File "..\RELEASE\HoMIAdmiN.exe"
  SetOutPath "$INSTDIR\Images"
  File /r "..\RELEASE\Images\*"
  
  SetOverwrite off
  SetOutPath "$INSTDIR\Config"
  File "..\RELEASE\Config\HoMIAdmiN.xml"
  
SectionEnd


Section "" HoMIDoM_SHORTCUTS

  SetOutPath "$INSTDIR"
  CreateDirectory $SMPROGRAMS\${PRODUCT_NAME}
  
  ; Création du raccourçi pour HomiAdmin
  ${If} ${SectionIsSelected} ${HoMIDoM_ADMIN}
    CreateShortCut $SMPROGRAMS\${PRODUCT_NAME}\HoMIAdmin.lnk $INSTDIR\HoMIAdmin.exe
  ${EndIf}
  
  ${If} "$optInstallAsService" == "0"
    CreateShortCut $SMPROGRAMS\${PRODUCT_NAME}\HoMIDoMService.lnk $INSTDIR\HoMIDoMService.exe
  ${EndIf}

SectionEnd


Section "" HoMIDoM_START

  ; Démarre le Servicce Windows
  StrCmp "$optInstallAsService" "1" 0 skipInstallService
  call InstallAndStartHomidomService
  
skipInstallService:


  
SectionEnd

; **********************************************************************
; Uninstall Section
; **********************************************************************
Section "Uninstall"

  SetOutPath "$INSTDIR"

  Banner::show /NOUNLOAD "Desintallation des service..."

  ExecWait "$INSTDIR\hitb.exe -sps" $0  ; stop-service
  DetailPrint "Arret du server (hitb -sps) : $0"

  ExecWait '"$INSTDIR\hitb.exe -ka"' $0
  DetailPrint "Arret du server (hitb -ka) : $0"

  ; desinstallation du service existant
  ExecWait '$INSTDIR\nssm.exe remove HoMIDoM confirm' $0
  DetailPrint "Desintallation du service : $0"

  Banner::destroy

  ; Remove registry keys
  DeleteRegKey HKLM "${PRODUCT_UNINSTALL_REGKEY}"
  DeleteRegKey HKLM "SOFTWARE\HoMIDoM"

  Delete $INSTDIR\*
  
  ;Delete $INSTDIR\Bdd\*
  ;RMDir "$INSTDIR\Bdd"
  
  ;Delete $INSTDIR\Config\*
  ;RMDir "$INSTDIR\Config"

  ;Delete $INSTDIR\Fichiers\*
  ;RMDir "$INSTDIR\Fichiers"

  Delete $INSTDIR\Images\*
  RMDir "$INSTDIR\Images"
  
  ;Delete $INSTDIR\Logs\*
  ;RMDir "$INSTDIR\Logs"

  ;Delete $INSTDIR\Templates\*
  ;RMDir "$INSTDIR\Templates"

  Delete "$INSTDIR\Drivers\*"
  ;Delete "$INSTDIR\Drivers\ZWave\*"
  ;Delete "$INSTDIR\Drivers\KNX\*"
  RMDir "$INSTDIR\Drivers\ZWave"
  RMDir "$INSTDIR\Drivers\KNX"
  RMDir "$INSTDIR\Drivers"


  
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\HoMIAdmin.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\HoMIDoMService.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_NAME}"
  
  ; Remove files and uninstaller
  Delete $INSTDIR\uninstall.exe

  ; Remove directories used
  RMDir "$INSTDIR"

SectionEnd


; **********************************************************************
; dé-installation du service windows via NSSM
; **********************************************************************
Function UnInstallHomidomService

  Banner::show /NOUNLOAD "Désinstallation du service..."

  ; desinstallation du service existant
  ExecCmd::exec /TIMEOUT=5000 '$PLUGINSDIR\nssm.exe remove HoMIDoM confirm'
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  DetailPrint "Desintallation du service : $0"

  Banner::destroy

FunctionEnd

; **********************************************************************
; installation du service windows via NSSM
; **********************************************************************
Function InstallHomidomService

  Banner::show /NOUNLOAD "Installation du service..."
  
  SetOutpath "$INSTDIR"
  File "tools\nssm-2.16\win32\nssm.exe"
  
  ; installation du service
  ;ExecCmd::exec /TIMEOUT=5000 '"$INSTDIR\nssm.exe"' 'install HoMIDoM "$INSTDIR\HoMIDomService.exe"'
  ;Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  ;DetailPrint "Installation du service : $0"
  
  ;MESSAGEBOX MB_ICONQUESTION "Installation du service ExecCmd: $0"
  ExecWait '"$INSTDIR\nssm.exe" install HoMIDoM "$INSTDIR\HoMIDomService.exe"' $0
  ;MESSAGEBOX MB_ICONQUESTION "Installation du service ExecWait: $0"
  ;MESSAGEBOX MB_ICONQUESTION "STOP"

  Banner::destroy

FunctionEnd

; **********************************************************************
; installation & démarrage du service windows
; **********************************************************************
Function InstallAndStartHomidomService

  call UnInstallHomidomService
  call InstallHomidomService
  
    ; vérif si install ok
    ; rc0 : install OK
    ; rc5 : error - or - already installed
    StrCmp "$0" "0" startWindowsService
      ; echec de l'installation => message
      MessageBox MB_ICONEXCLAMATION "L'installation du service à échouée."
      goto exit_InstallAndStartHomidomService

startWindowsService:

  ; Démarrage & Redémarrage su service le temps de trouver pourquoi le Serveur ne démarre
  ; pas correctement s'il detecte un mise à jour (affichage de la la page web)

  Banner::show /NOUNLOAD "Démarrage du service..."
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -sts"' ""
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  DetailPrint "Démarrage du service : $0"

  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -sps"' ""
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).

  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -sts"' ""
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  DetailPrint "Redémarrage du service : $0"

  
  Banner::destroy
  
exit_InstallAndStartHomidomService:

FunctionEnd

; **********************************************************************
; Kill de toutes les application & services HoMIDom
; **********************************************************************
Function KillAllHomidomServices

  Banner::show /NOUNLOAD "Arret d'HoMIDom en  cours ..."
  
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -sps"' "" ; stop-service
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  DetailPrint "Arret du server (hitb -sps) : $0"
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -ka"' ""  ; kill-all
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  DetailPrint "Arret du server (hitb -ka) : $0"
  
  Banner::destroy
  
FunctionEnd

Function nsDialogsPage
	nsDialogs::Create 1018
	Pop $Dialog

	${If} $Dialog == error
		Abort
	${EndIf}

        StrCpy $optInstallAsService "0"
        StrCpy $optCreateStartMenuShortcuts "1"
        StrCpy $optStartService "0"

        ${NSD_CreateCheckbox} 0 10u 100% 10u "Créer les raccourçis dans le menu Démarrer"
	Pop $chkCreateStartMenuShortcuts_Handle
	${If} $optCreateStartMenuShortcuts == ${BST_CHECKED}
		${NSD_Check} $chkCreateStartMenuShortcuts_Handle
	${EndIf}

  ${If} ${SectionIsSelected} ${HoMIDoM_SERVICE}

        ${NSD_CreateCheckbox} 0 30u 100% 10u "Installer HoMIDoM Server en tant que Service Windows"
	Pop $chkInstallAsService_Handle
	${If} $optInstallAsService == ${BST_CHECKED}
		${NSD_Check} $chkInstallAsService_Handle
	${EndIf}

        ${NSD_CreateCheckbox} 0 50u 100% 10u "Demarrer HoMIDoM Server à la fin de l'installation"
	Pop $chkStartService_Handle
	${If} $optStartService == ${BST_CHECKED}
		${NSD_Check} $chkStartService_Handle
	${EndIf}
	
  ${EndIf}


	nsDialogs::Show
FunctionEnd

Function nsDialogsPageLeave
         ${NSD_GetState} $chkInstallAsService_Handle $optInstallAsService
         ${NSD_GetState} $chkStartService_Handle $optStartService
         ${NSD_GetState} $chkCreateStartMenuShortcuts_Handle $optCreateStartMenuShortcuts
FunctionEnd
