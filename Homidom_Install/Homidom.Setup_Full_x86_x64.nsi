; **********************************************************************
; Homidom Server Install & Update script
; **********************************************************************

!define PRODUCT_NAME "HoMIDoM"
!define PRODUCT_VERSION "#PRODUCT_VERSION#"
!define PRODUCT_BUILD "#PRODUCT_BUILD#"
!define PRODUCT_REVISION "#PRODUCT_REVISION#"
!define PRODUCT_RELEASE "RELEASE"
!define PRODUCT_PUBLISHER "HoMIDoM"
!define PRODUCT_WEB_SITE "http://www.homidom.fr"

!define PRODUCT_DIR_REGKEY "Software\${PRODUCT_NAME}"
!define PRODUCT_UNINSTALL_REGKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"

; Version du Framework .NET requis et URL de téléchargement
!define DOT_MAJOR "4"
!define DOT_MINOR "0"
!define URL_DOTNET "http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe"
!include "includes\CheckDotNetInstalled.nsh"

; Dossier racine
!define ROOT_DIR ".."
; Dossier de publication => dossier contenant les binaires
!define PUBLISH_DIR "${ROOT_DIR}\${PRODUCT_RELEASE}"
; Dossier de destination du package généré
!define PACKAGES_DIR "Packages"

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

; **********************************************************************
; Custom Options
; **********************************************************************
Var Dialog
var chkInstallAsService_Handle
Var optInstallAsService
var chkStartService_Handle
Var optStartService
Var chkCreateStartMenuShortcuts_Handle
Var optCreateStartMenuShortcuts

Function nsDialogsPage
	nsDialogs::Create 1018
	Pop $Dialog
	
	${If} $Dialog == error
		Abort
	${EndIf}

        StrCpy $optInstallAsService "0"
        StrCpy $optCreateStartMenuShortcuts "1"
        StrCpy $optStartService "0"

        ${NSD_CreateCheckbox} 0 10u 100% 10u "Installer HoMIDoM Server en tant que Service Windows"
	Pop $chkInstallAsService_Handle
	${If} $optInstallAsService == ${BST_CHECKED}
		${NSD_Check} $chkInstallAsService_Handle
	${EndIf}

        ${NSD_CreateCheckbox} 0 30u 100% 10u "Demarrer HoMIDoM Server à la fin de l'installation"
	Pop $chkStartService_Handle
	${If} $optStartService == ${BST_CHECKED}
		${NSD_Check} $chkStartService_Handle
	${EndIf}

        ${NSD_CreateCheckbox} 0 50u 100% 10u "Créer les raccourçis dans le menu Démarrer"
	Pop $chkCreateStartMenuShortcuts_Handle
	${If} $optCreateStartMenuShortcuts == ${BST_CHECKED}
		${NSD_Check} $chkCreateStartMenuShortcuts_Handle
	${EndIf}
	
	nsDialogs::Show
FunctionEnd

Function nsDialogsPageLeave
         ${NSD_GetState} $chkInstallAsService_Handle $optInstallAsService
         ${NSD_GetState} $chkStartService_Handle $optStartService
         ${NSD_GetState} $chkCreateStartMenuShortcuts_Handle $optCreateStartMenuShortcuts
FunctionEnd



; Request application privileges for Windows Vista
RequestExecutionLevel admin

Var IsHomidomInstalled ;indique si HoMIDom est déja installé
Var IsHomidomInstalledAsService ;indique si HoMIDoMService est installé en tant que service windows
Var IsHomidomServiceRunning ;indique si le service HoMIDoMService est démarré
Var IsHomidomAppRunning ;indique si une app Homidom est en cours d'execution (Admin,WPF, )
Var HomidomInstalledVersion ;Numéro de version Homidom.dll

; **********************************************************************
; Initialisation
; **********************************************************************
Function .onInit

  # the plugins dir is automatically deleted when the installer exits
  InitPluginsDir
  File /oname=$PLUGINSDIR\splash.bmp "..\Images\logo\logo.bmp"
  ; déploiement de NSSM & HITB
  File /oname=$PLUGINSDIR\nssm.exe "tools\nssm-2.16\win32\nssm.exe"
  File /oname=$PLUGINSDIR\hitb.exe "tools\hitb-1.0\win32\hitb.exe"
    
  # affichage du Logo "splah"
  advsplash::show 2000 600 400 -1 $PLUGINSDIR\splash
  Pop $0

  ; sélection de la langue
  !insertmacro MUI_LANGDLL_DISPLAY
  

  ; Vérification de la version du Framework .NET
  Call CheckDotNetInstalled
  
  ; détécction de l'OS(32bits/64bits) & pré-configuration du dossier de destination
  StrCpy $instdir "$programfiles32\${PRODUCT_NAME}"
  ${If} ${RunningX64}
        StrCpy $instdir "$programfiles64\${PRODUCT_NAME}"
  ${EndIf}
  
  ; Detection d'un installation existante
  ; basé sur la présence de la DLL principale dans le dossier d'install par défaut -ou- spécifié en BdR si déja installé
  StrCpy $IsHomidomInstalled "0"
  IfFileExists "$INSTDIR\HoMIDom.dll" 0 homidomIsNotInstalled
    StrCpy $IsHomidomInstalled "1"
    ;récupération de la version installée
    ${GetFileVersion} "$INSTDIR\HoMIDom.dll" $HomidomInstalledVersion

homidomIsNotInstalled:

    ; détéction du type d'installation du server (service windows ou pas)
    ; 0: Not found
    ; 1: Found

    ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -cis"' ""
    Pop $IsHomidomInstalledAsService ; return value - process exit code or error or STILL_ACTIVE (0x103).
    StrCpy $optInstallAsService $IsHomidomInstalledAsService
    
FunctionEnd

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

  ; - Démarrage du serveur
  StrCmp "$optStartService" "0" skipStartService
  StrCmp "$optInstallAsService" "1" skipStartService
    SetOutPath "$INSTDIR"
    Exec '"$INSTDIR\HomidomService.exe"'

skipStartService:

FunctionEnd


SectionGroup "HoMIDoM" HoMIDoM_GRP

Section "" HoMIDoM_STOPSVC

  Banner::show /NOUNLOAD ""

    ; Vérifie si le service Homidom est démarré
    ; 0: Not running or Not found
    ; 1: Found and Running
    ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -crs"' ""
    Pop $IsHomidomServiceRunning ; return value - process exit code or error or STILL_ACTIVE (0x103).

    ; vérifie si des app Homidom sont en cours d'execution
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
  StrCmp "$IsHomidomInstalledAsService" "1" 0 extStopSvcSection
  StrCmp "$optInstallAsService" "0" 0 extStopSvcSection
    call UnInstallHomidomService
extStopSvcSection:

SectionEnd


Section "HoMIDoM Framework" HoMIDoM_FRAMEWORK

  ; Fichiers de base : requis par l'ensemble des applications de la solution
  ; Section obligatoire.
  SectionIn RO

  CreateDirectory "$INSTDIR"
  CreateDirectory "$INSTDIR\Bdd"
  CreateDirectory "$INSTDIR\Config"
  CreateDirectory "$INSTDIR\Drivers"
  CreateDirectory "$INSTDIR\Fichiers"
  CreateDirectory "$INSTDIR\Images"
  CreateDirectory "$INSTDIR\Logs"
  CreateDirectory "$INSTDIR\Templates"
  
  SetOutPath "$INSTDIR"
  File "tools\hitb-1.0\win32\hitb.exe"
  File "..\RELEASE\HoMIDom.DLL"
  File "..\RELEASE\NCrontab.dll"
    
  ${If} ${RunningX64}
    File /r /x "*.xml" /x "*.pdb" /x "MjpegProcessor.*" "..\Dll_externes\Homidom-64bits\*"
  ${Else}
    File /r /x "*.xml" /x "*.pdb" /x "MjpegProcessor.*" "..\Dll_externes\Homidom-32bits\*"
  ${EndIf}

  SetOutPath "$INSTDIR\Templates"
  File /r "..\RELEASE\Templates\*"

SectionEnd

Section "HoMIDoM Service" HoMIDoM_SERVICE

  SectionIn RO

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
  
SectionEnd

Section "HoMIDoM Admin" HoMIDoM_ADMIN
  
  SectionIn RO

  SetOverwrite on
  SetOutPath "$INSTDIR"
  File "..\RELEASE\HoMIAdmiN.exe"
  SetOutPath "$INSTDIR\Images"
  File /r "..\RELEASE\Images\*"
  
  SetOverwrite off
  SetOutPath "$INSTDIR\Config"
  File "..\RELEASE\Config\HoMIAdmiN.xml"
  
SectionEnd

SectionGroupEnd

SectionGroup "Drivers" DRIVERS_GRP

  Section "Arduino" DRIVER_ARDUINO
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Arduino.dll"
  SectionEnd

  Section "CurrentCost" DRIVER_CURRENTCOST
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_CurrentCost.dll"
  SectionEnd
  
  Section "Arduino" DRIVER_DMX
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_DMX.dll"
  SectionEnd

  Section "GSM" DRIVER_FOOBAR
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Foobar.dll"
  SectionEnd

  Section "Foobar" DRIVER_GSM
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Gsm.dll"
  SectionEnd

  Section "HomeSeer" DRIVER_HS
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_HomeSeer.dll"
  SectionEnd

  Section "HTTP" DRIVER_HTTP
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_http.dll"
  SectionEnd

  SectionGroup "Velleman Kxxxx" DRIVERS_VELLEMAN
    Section "K8000" DRIVER_K8000
      SetOutPath "$INSTDIR\Drivers"
      File "..\RELEASE\Drivers\Driver_k8000.dll"
    SectionEnd
    Section "K8055" DRIVER_K8055
      SetOutPath "$INSTDIR\Drivers"
      File "..\RELEASE\Drivers\Driver_k8055.dll"
    SectionEnd
    Section "K8056" DRIVER_K8056
      SetOutPath "$INSTDIR\Drivers"
      File "..\RELEASE\Drivers\Driver_k8056.dll"
    SectionEnd
  SectionGroupEnd
  
  Section "KNX/EIBD" DRIVER_KNX
    CreateDirectory "$INSTDIR\Drivers\KNX"
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_KNX.dll"
  SectionEnd
  
  Section "Mirror" DRIVER_MIRROR
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Mirror.dll"
  SectionEnd

  Section "OneWire" DRIVER_1WIRE
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_onewire.dll"
  SectionEnd
  
  SectionGroup "OpenWebNet" DRIVERS_OpenWebNet
    Section "OpenWebNet IP" DRIVER_OpenWebNet_IP
      SetOutPath "$INSTDIR\Drivers"
      File "..\RELEASE\Drivers\Driver_OpenWebNet_IP.dll"
    SectionEnd
    Section "OpenWebNet USB" DRIVER_OpenWebNet_USB
      SetOutPath "$INSTDIR\Drivers"
      File "..\RELEASE\Drivers\Driver_OpenWebNet_USB.dll"
    SectionEnd
  SectionGroupEnd
  
  Section "Phidget" DRIVER_Phidget
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Phidget.dll"
  SectionEnd

  Section "PLCBUS" DRIVER_PLCBUS
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_PLCBUS.dll"
  SectionEnd
  
  Section "RFXMitter" DRIVER_RFXMitter
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_RFXMitter.dll"
  SectionEnd
  
  Section "RFXReceiver" DRIVER_RFXReceiver
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_RFXReceiver.dll"
  SectionEnd

  Section "RFXtrx" DRIVER_RFXtrx
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_RFXtrx.dll"
  SectionEnd
  
  Section "System" DRIVER_System
    SectionIn RO
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_System.dll"
  SectionEnd
  Section "teleinfo" DRIVER_teleinfo
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_teleinfo.dll"
  SectionEnd
  Section "Teracom" DRIVER_Teracom
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Teracom.dll"
  SectionEnd
  Section "USBuirt" DRIVER_USBuirt
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_USBuirt.dll"
  SectionEnd

  SectionGroup "X10" DRIVERS_X10
    Section "X10 CM11" DRIVER_X10_CM11
      SetOutPath "$INSTDIR\Drivers"
      File "..\RELEASE\Drivers\Driver_X10_CM11.dll"
    SectionEnd
    Section "X10 CM15" DRIVER_X10_CM15
      SetOutPath "$INSTDIR\Drivers"
      File "..\RELEASE\Drivers\Driver_X10_CM15.dll"
    SectionEnd
  SectionGroupEnd

  Section "Virtuel" DRIVER_Virtuel
    SectionIn RO
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Virtuel.dll"
  SectionEnd
  Section "Weather" DRIVER_Weather
    SectionIn RO
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Weather.dll"
  SectionEnd
  Section "Zibase" DRIVER_Zibase
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Zibase.dll"
  SectionEnd
  Section "Zwave" DRIVER_Zwave
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Zwave.dll"
    CreateDirectory "$INSTDIR\Drivers\ZWave"
    SetOutPath "$INSTDIR\Drivers\ZWave"
    File /r "..\RELEASE\Drivers\Zwave\*"
  SectionEnd
  
  
SectionGroupEnd


Section "" HoMIDoM_SHORTCUTS

  SetOutPath "$INSTDIR"
  CreateDirectory $SMPROGRAMS\${PRODUCT_NAME}
  CreateShortCut $SMPROGRAMS\${PRODUCT_NAME}\HoMIAdmin.lnk $INSTDIR\HoMIAdmin.exe

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