; **********************************************************************
; Homidom Install & Update script
; **********************************************************************

; --- définition des constantes de base
!define PRODUCT_NAME "HoMIDoM"
!define PRODUCT_VERSION "#PRODUCT_VERSION#"
!define PRODUCT_BUILD "#PRODUCT_BUILD#"
!define PRODUCT_REVISION "#PRODUCT_REVISION#"
!define PRODUCT_RELEASE "#PRODUCT_RELEASE#"
!define PRODUCT_PUBLISHER "HoMIDoM"
!define PRODUCT_WEB_SITE "http://www.homidom.com"
!define PRODUCT_DIR_REGKEY "Software\${PRODUCT_NAME}"
!define PRODUCT_UNINSTALL_REGKEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"

!define DEFAULT_CFG_IPSOAP "localhost"
!define DEFAULT_CFG_PORTSOAP "7999"
!define DEFAULT_CFG_IDSRV "123456789"

; --- définition des constantes "Special Folders"
;!define CSIDL_DESKTOP '0x0' ;Desktop path
;!define CSIDL_PROGRAMS '0x2' ;Programs path
;!define CSIDL_PERSONAL '0x5' ;My document path
;!define CSIDL_FAVORITES '0x6' ;Favorites path
;!define CSIDL_STARTUP '0x7' ;Startup path
;!define CSIDL_RECENT '0x8' ;Recent documents path
;!define CSIDL_SENDTO '0x9' ;Sendto documents path
;!define CSIDL_STARTMENU '0xB' ;StartMenu path
;!define CSIDL_MUSIC '0xD' ;My Music path
;!define CSIDL_DESKTOPDIR '0x10' ;Desktop Directory path
;!define CSIDL_COMPUTER '0x11' ;My Computer path
;!define CSIDL_FONTS '0x14' ;Fonts directory path
;!define CSIDL_TEMPLATES '0x15' ;Windows Template path
;!define CSIDL_APPDATA '0x1A' ;Application Data path
;!define CSIDL_LOCALAPPDATA '0x1C' ;Local Application Data path
;!define CSIDL_INTERNETCACHE '0x20' ;Internet Cache path
;!define CSIDL_COOKIES '0x21' ;Cookies path
;!define CSIDL_HISTORY '0x22' ;History path
!define CSIDL_COMMONAPPDATA '0x23' ;Common Application Data path
;!define CSIDL_SYSTEM '0x25' ;System path
;!define CSIDL_PROGRAMFILES '0x26' ;Program Files path
;!define CSIDL_MYPICTURES '0x27' ;My Pictures path
;!define CSIDL_COMMONPROGRAMFILES '0x2B' ;Common Program Files path

;Var SF_DESKTOP
;Var SF_PROGRAMS
;Var SF_PERSONAL
;Var SF_FAVORITES
;Var SF_STARTUP
;Var SF_RECENT
;Var SF_SENDTO
;Var SF_STARTMENU
;Var SF_MUSIC
;Var SF_DESKTOPDIR
;Var SF_COMPUTER
;Var SF_FONTS
;Var SF_TEMPLATES
;Var SF_APPDATA
;Var SF_LOCALAPPDATA
;Var SF_INTERNETCACHE
;Var SF_COOKIES
;Var SF_HISTORY
Var SF_COMMONAPPDATA
;Var SF_SYSTEM
;Var SF_PROGRAMFILES
;Var SF_MYPICTURES
;Var SF_COMMONPROGRAMFILES

##############################################################################################
# URL : https://msdn.microsoft.com/en-us/library/8z6watww(v=vs.110).aspx                         #
#                                                                                                #
# Versions préinstallée et installable en fonction de l'OS :                                     #
#                                                                                                #
# Windows 10 Insider Preview Build : Pré-Inst : 4.6.2 Preview                                    #
# Windows 10 (Upd Nov 2015)        : Pré-Inst : 4.6.1 - Version-Max 4.6.1 & 4.6.2 Preview        #
# Windows 10 (Upd Nov 2015)        : Pré-Inst : 4.6.1 - Version-Max 4.6.1 & 4.6.2 Preview        #
# Windows 10                       : Pré-Inst : 4.6   - Version-Max 4.6.1                        #
# Windows 8.1                      : Pré-Inst : 4.5.1 - Version-Max 4.6.1                        #
# Windows 8                        : Pré-Inst : 4.5   - Version-Max 4.6.1 (Dernière version) !!! #
# Windows 7 SP1                    : Pré-Inst : néant - Version-Max 4.6.1                        #
# Windows Vista SP2                : Pré-Inst : néant - Version-Max 4.6   (Dernière version) !!! #
# Windows XP                       : Pré-Inst : néant - Version-Max 4.0   (Dernière version) !!! #
#                                                                                                #
# Windows Server 2012 R2           : Pré-Inst : 4.5.1 - Version-Max 4.6.1                        #
# Windows Server 2012 (Edt 64Bits) : Pré-Inst : 4.5   - Version-Max 4.6.1                        #
# Windows Server 2008 R2 SP1       : Pré-Inst : néant - Version-Max 4.6.1                        #
# Windows Server 2008 R2           : Pré-Inst : néant - Version-Max 4.6   (Dernière version) !!! #
# Windows Server 2003 R2           : Pré-Inst : néant - Version-Max 4.0   (Dernière version) !!! #
#                                                                                                #
##################################################################################################

; --- Version du Framework .NET requis et URL de téléchargement
!define DOT_MAJOR "4"
!define DOT_MINOR "0"
;!define URL_DOTNET "https://download.microsoft.com/download/1/6/7/167F0D79-9317-48AE-AEDB-17120579F8E2/NDP451-KB2858728-x86-x64-AllOS-ENU.exe"
;!define URL_DOTNET "https://download.microsoft.com/download/C/3/A/C3A5200B-D33C-47E9-9D70-2F7C65DAAD94/NDP46-KB3045557-x86-x64-AllOS-ENU.exe"
!define URL_DOTNET "https://download.microsoft.com/download/E/4/1/E4173890-A24A-4936-9FC9-AF930FE3FA40/NDP461-KB3102436-x86-x64-AllOS-ENU.exe"

; --- Dossier racine contenant les sources (relatif à l'emplacement du script NSIS)
!define ROOT_DIR ".."

; --- Dossier de publication => dossier contenant les binaires
!define PUBLISH_DIR "${ROOT_DIR}\\${PRODUCT_RELEASE}\"

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

!include "MUI2.nsh"
!include "Sections.nsh"
!include "TextFunc.nsh"
!include "LogicLib.nsh"
!include "includes\x64.nsh"
!include "nsDialogs.nsh"
!include "includes\CustomLog.nsh"

; --- inclusion du code de détection et de téléchargement du Framework .NET
!include "includes\CheckDotNetInstalled.nsh"

; --- inclusion du code de détection et de téléchargement des runtime Visual Studio 2010, 2012, 2013
!include "includes\Redist.VC10.nsh"
!include "includes\Redist.VC11.nsh"
!include "includes\Redist.VC12.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_COMPONENTSPAGE_NODESC
!define MUI_ICON "${ROOT_DIR}\Images\Logo\icone.ico"
!define MUI_UNICON "${ROOT_DIR}\Images\Logo\icone.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_HEADERIMAGE_BITMAP "bandeau.bmp" ; 150x57
!define MUI_WELCOMEFINISHPAGE_BITMAP "Setup.bmp" ; 164x314
;!define MUI_COMPONENTSPAGE_SMALLDESC ;No value
;!define MUI_INSTFILESPAGE_COLORS "000000 FFFFFF"
; Background color
;!define MUI_BGCOLOR  3F3F3F ; This is the background color of the installer main body.
;!define MUI_LICENSEPAGE_BGCOLOR 3F3F3F
;!define MUI_DIRECTORYPAGE_BGCOLOR 3F3F3F
;!define MUI_STARTMENUPAGE_BGCOLOR 3F3F3F
;!define MUI_INSTFILESPAGE_COLORS 3F3F3F

;!define UMUI_TEXT_LIGHTCOLOR  FFFFFF ; Font color of Titles like the "Description" in the components screen and "Destination
!define TEMP1 $R0 ;Temp variable

; Welcome page
!insertmacro MUI_PAGE_WELCOME
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
;!insertmacro MUI_PAGE_FINISH


;!insertmacro MUI_UNPAGE_CONFIRM
!define MUI_PAGE_CUSTOMFUNCTION_SHOW un.ModifyUnWelcome
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE un.LeaveUnWelcome
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_INSTFILES
Var FullUninstBox
Var FullUninstBox_State

; Language files
!include "includes\localization.fr.nsh"
;!include "includes\localization.en.nsh"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}.${PRODUCT_BUILD} (${PRODUCT_REVISION})"
OutFile "${PACKAGES_DIR}\${PRODUCT_NAME}.Setup.${PRODUCT_VERSION}.${PRODUCT_BUILD}.${PRODUCT_REVISION}_Full_x86_x64_${PRODUCT_RELEASE}.exe"
InstallDir "" ;"$PROGRAMFILES\HoMIDoM"
BrandingText "HoMIDoM Installer v3.1"
;InstallDirRegKey HKLM "SOFTWARE\${PRODUCT_NAME}" "InstallDir"
ShowInstDetails show
ShowUnInstDetails show

; --- définition des variables de la boite de dialogue personalisé 
Var Dialog
Var chkInstallAsService_Handle
Var optInstallAsService
Var chkStartService_Handle
Var optStartService
Var chkStartServiceGUI_Handle
Var optStartServiceGUI
Var chkCreateStartMenuShortcuts_Handle
Var optCreateStartMenuShortcuts
Var txtCfgIpSoap_Handle
Var txtCfgPortSoap_Handle
Var txtCfgIdSrv_Handle

; Request application privileges for Windows Vista
RequestExecutionLevel admin

Var platform
Var IsComposantChecked

; --- définition des variables personalisées
Var IsHomidomInstalled ;indique si HoMIDom est déja installé
Var IsHomidomInstalledAsService ;indique si HoMIServicE est installé en tant que service windows
Var IsHomidomInstalledAsServiceNSSM
;Var IsHomidomServiceRunning ;indique si le service HoMIServicE est démarré
;Var IsHomidomAppRunning ;indique si une app Homidom est en cours d'execution (Admin,WPF, )
Var HomidomInstalledVersion ;Numéro de version Homidom.dll
Var IsPreviousInstallIsDeprecated ;Indique si la précedente installation a été faite avec l'ancien installeur (msi)
;Var HomidomServiceVersion ;Indique le type de service installé : 1=> HomidomSvc via NSSM, 2=> HomiService
Var HomiServiceName

Var IsWindowsFirewallEnabled

Var cfg_ipsoap
Var cfg_portsoap
Var cfg_idsrv

!define Unicode2Ansi "!insertmacro Unicode2Ansi"
!macro Unicode2Ansi String outVar
  System::Call 'kernel32::WideCharToMultiByte(i 0, i 0, w "${String}", i -1, t .s, i ${NSIS_MAX_STRLEN}, i 0, i 0) i'
  Pop "${outVar}"
!macroend

; ################################################################
; appends \ to the path if missing
; example: !insertmacro GetCleanDir "c:\blabla"
; Pop $0 => "c:\blabla\"
!macro GetCleanDir INPUTDIR
  ; ATTENTION: USE ON YOUR OWN RISK!
  ; Please report bugs here: http://stefan.bertels.org/
  !define Index_GetCleanDir 'GetCleanDir_Line${__LINE__}'
  Push $R0
  Push $R1
  StrCpy $R0 "${INPUTDIR}"
  StrCmp $R0 "" ${Index_GetCleanDir}-finish
  StrCpy $R1 "$R0" "" -1
  StrCmp "$R1" "\" ${Index_GetCleanDir}-finish
  StrCpy $R0 "$R0\"
${Index_GetCleanDir}-finish:
  Pop $R1
  Exch $R0
  !undef Index_GetCleanDir
!macroend

; ################################################################
; similar to "RMDIR /r DIRECTORY", but does not remove DIRECTORY itself
; example: !insertmacro RemoveFilesAndSubDirs "$INSTDIR"
!macro RemoveFilesAndSubDirs DIRECTORY
  ; ATTENTION: USE ON YOUR OWN RISK!
  ; Please report bugs here: http://stefan.bertels.org/
  !define Index_RemoveFilesAndSubDirs 'RemoveFilesAndSubDirs_${__LINE__}'

  Push $R0
  Push $R1
  Push $R2

  !insertmacro GetCleanDir "${DIRECTORY}"
  Pop $R2
  FindFirst $R0 $R1 "$R2*.*"
${Index_RemoveFilesAndSubDirs}-loop:
  StrCmp $R1 "" ${Index_RemoveFilesAndSubDirs}-done
  StrCmp $R1 "." ${Index_RemoveFilesAndSubDirs}-next
  StrCmp $R1 ".." ${Index_RemoveFilesAndSubDirs}-next
  IfFileExists "$R2$R1\*.*" ${Index_RemoveFilesAndSubDirs}-directory
  ; file
  Delete "$R2$R1"
  goto ${Index_RemoveFilesAndSubDirs}-next
${Index_RemoveFilesAndSubDirs}-directory:
  ; directory
  RMDir /r "$R2$R1"
${Index_RemoveFilesAndSubDirs}-next:
  FindNext $R0 $R1
  Goto ${Index_RemoveFilesAndSubDirs}-loop
${Index_RemoveFilesAndSubDirs}-done:
  FindClose $R0

  Pop $R2
  Pop $R1
  Pop $R0
  !undef Index_RemoveFilesAndSubDirs
!macroend

; **********************************************************************
; Initialisation - code exécuté avant l'affichage de la 1ere fenêtre
; **********************************************************************
Function .onInit

  !insertmacro Log_Init "$EXEDIR\$EXEFILE.log"
  !insertmacro Log_String "[Function .onInit]"
  !insertmacro Log_String "================================================================================"
  !insertmacro Log_String " ${PRODUCT_NAME} v${PRODUCT_VERSION}.${PRODUCT_BUILD}.${PRODUCT_REVISION}"
  !insertmacro Log_String "================================================================================"
  !insertmacro Log_String "Dossier d'installation : $INSTDIR"

  ; -- Création du dossier temporaires nécessaires à l'installeur (%TEMP%\ns*****)
  InitPluginsDir

  ; --- déploiement des fichiers nécessaires à l'installation
  File /oname=$PLUGINSDIR\nssm.exe "tools\nssm-2.16\win32\nssm.exe"
  File /oname=$PLUGINSDIR\hitb.exe "tools\hitb-1.0\win32\hitb.exe"

  # --- affichage du Logo "splah"
  ;File /oname=$PLUGINSDIR\splash.bmp "..\Images\logo\logo.bmp"
  ;advsplash::show 2000 600 400 -1 $PLUGINSDIR\splash
  ;Pop $0

  ; --- sélection de la langue
  !insertmacro MUI_LANGDLL_DISPLAY

  ; --- Vérification de la version du Framework .NET
  !insertmacro Log_String "Vérification de la version du Framework .NET"
  Call CheckDotNetInstalled

  ; --- détection de l'OS (32bits/64bits) & pré-configuration du dossier de destination
  ${If} ${RunningX64}
    !insertmacro Log_String "Architecture 64-bit detectée."
    SetRegView 64
    StrCpy $platform "x64"
    ReadRegStr $INSTDIR HKLM "SOFTWARE\${PRODUCT_NAME}" "InstallDir"
    ${If} $INSTDIR == ""
      StrCpy $INSTDIR "$programfiles64\${PRODUCT_NAME}"
    ${Else}
      !insertmacro Log_String "Détection d'une installation antérieure : $INSTDIR"
    ${EndIf}
  ${Else}
    !insertmacro Log_String "Architecture 32-bit detectée."
    SetRegView 32
    StrCpy $platform "x86"
    ReadRegStr $INSTDIR HKLM "SOFTWARE\${PRODUCT_NAME}" "InstallDir"
    ${If} $INSTDIR == ""
      StrCpy $INSTDIR "$programfiles32\${PRODUCT_NAME}"
    ${Else}
      !insertmacro Log_String "Détection d'une installation antérieure : $INSTDIR"
    ${EndIf}
  ${EndIf}

  !insertmacro Log_String "Dossier d'installation : $INSTDIR"

  ; --- Vérification de la présence des runtimes VC++
  !insertmacro Log_String "Vérification de la présence des runtimes VC++ 2010 (10.0)"
  Call CheckVC10Redist
  !insertmacro Log_String "Vérification de la présence des runtimes VC++ 2012 (11.0)"
  Call CheckVC11Redist
  !insertmacro Log_String "Vérification de la présence des runtimes VC++ 2013 (12.0)"
  Call CheckVC12Redist

  !insertmacro Log_String "Le dossier d'installation par défaut est '$INSTDIR'."
  
  ; --- Initialisation des variables
  StrCpy $IsHomidomInstalled "0"
  StrCpy $IsPreviousInstallIsDeprecated "0"
  StrCpy $HomiServiceName "HoMIServicE"
  StrCpy $IsWindowsFirewallEnabled "0"
  StrCpy $cfg_ipsoap "${DEFAULT_CFG_IPSOAP}"
  StrCpy $cfg_portsoap "${DEFAULT_CFG_PORTSOAP}"
  StrCpy $cfg_idsrv "${DEFAULT_CFG_IDSRV}"

  ; --- Recherche d'une installation faite avec l'ancien installeur
  ;!insertmacro Log_String "Recherche des installation existante en base de registre."
  StrCpy $0 0
  ${Do}
    ; parcours de chaque entrée 'Uninstall" du registre
    EnumRegKey $1 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall" $0
    ${If} $1 != ""
      IntOp $0 $0 + 1
      ; Lecture du "nom du produit"
      ReadRegStr $2 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$1" "DisplayName"
      ${If} $2 == "HoMIDoM"
        ; entrée trouvée
        ReadRegDword $3 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$1" "WindowsInstaller"
        !insertmacro Log_String "Une installation existante a été trouvée dans la clé de registre 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$1'"
        ${If} $3 == 1
          MessageBox MB_ICONEXCLAMATION|MB_OK "Une ancienne installation a été détectée. Veuillez effectuer une sauvegarde de vos données, désinstaller manuellement l'ancienne version et relancer une nouvelle installation."
          Abort
        ${EndIf}
      ${EndIf}
    ${EndIf}
  ${LoopUntil} $1 == ""

  ; --- Détection basée sur la présence de la DLL principale dans le dossier d'install par défaut -ou- spécifié en BdR si déja installé
  !insertmacro Log_String "Recherche d'une installation exitante dans le dossier '$INSTDIR'."
  ${If} ${FileExists} "$INSTDIR\HoMIDom.dll"
    ; --- Installation existante détectée !
    StrCpy $IsHomidomInstalled "1"
    ; --- récupération de la version installée => $HomidomInstalledVersion
    ${GetFileVersion} "$INSTDIR\HoMIDom.dll" $HomidomInstalledVersion
    !insertmacro Log_String "Une installation existante a été trouvé dans le dossier '$INSTDIR'. Le numéro de version est '$HomidomInstalledVersion'"
  ${EndIf}

  !insertmacro Log_String "Recherche des services HoMIDoM..."
  ; Vérification de l'existance du service V1
  SimpleSC::ExistsService "HoMIDoM"
  Pop $0
  ${If} $0 == "0"
    StrCpy $IsHomidomInstalledAsServiceNSSM "1"
    StrCpy $optInstallAsService "1"
    !insertmacro Log_String "Une version du service HoMIDoM installée avec NSSM a été trouvée."
  ${EndIf}
  
  ; Vérification de l'existance du service V2
  SimpleSC::ExistsService "$HomiServiceName"
  Pop $0 ; returns an errorcode if the service doesn´t exists (<>0)/service exists (0)
  ${If} $0 == 0
    StrCpy $IsHomidomInstalledAsService "1"
    StrCpy $optInstallAsService "1"
    StrCpy $optStartService "1"
    StrCpy $optStartServiceGUI "1"
    !insertmacro Log_String "Une version du service HoMIDoM installée avec InstallUtil a été trouvée."
  ${EndIf}

  ; Check if the firewall is enabled
  SimpleFC::IsFirewallEnabled
  Pop $0 ; return error(1)/success(0)
  Pop $1 ; return 1=Enabled/0=Disabled
  ${If} $0 == 0
    StrCpy $IsWindowsFirewallEnabled "$1"
    ${If} $IsWindowsFirewallEnabled == 1
      !insertmacro Log_String "Le pare-feux Windows est activé."
    ${Else}
      !insertmacro Log_String "Le pare-feux Windows n'est pas activé."
    ${EndIf}
  ${EndIf}

  Call LoadConfig
  !insertmacro Log_String "Dossier d'installation : $INSTDIR"

  !insertmacro Log_String "IsHomidomInstalled: $IsHomidomInstalled"
  !insertmacro Log_String "HomidomInstalledVersion: $HomidomInstalledVersion"
  !insertmacro Log_String "IsHomidomInstalledAsServiceNSSM: $IsHomidomInstalledAsServiceNSSM"
  !insertmacro Log_String "IsHomidomInstalledAsService: $IsHomidomInstalledAsService"
    
  !insertmacro Log_String "================================================================================"
  !insertmacro Log_String "Début de l'installation."
    
FunctionEnd

Function LoadConfig
!insertmacro Log_String "[Function LoadConfig]"

;  StrCpy $cfg_ipsoap ${DEFAULT_CFG_IPSOAP}
;  StrCpy $cfg_portsoap ${DEFAULT_CFG_PORTSOAP}
;  StrCpy $cfg_idsrv ${DEFAULT_CFG_IDSRV}

  ; Vérification de la présence du fichier de configuration
  !insertmacro Log_String "Recherche du fichier de configuration."
  ${If} ${FileExists} "$INSTDIR\Config\HoMIDom.xml"
  !insertmacro Log_String "-- fichier trouvé, récupération des informations."
    nsisXML::create
    nsisXML::load "$INSTDIR\Config\HoMIDom.xml"
    ; récupération du noeud /homidom/server
    nsisXML::select '/homidom/server'
    ${If} $2 != 0
      nsisXML::getAttribute "ipsoap"
      ${If} $3 != ""
        StrCpy $cfg_ipsoap $3
        !insertmacro Log_String "-- ipsoap = $cfg_ipsoap"
      ${EndIf}
      nsisXML::getAttribute "portsoap"
      ${If} $3 != ""
        StrCpy $cfg_portsoap $3
        !insertmacro Log_String "-- PortSoap = $cfg_portsoap"
      ${EndIf}
      nsisXML::getAttribute "idsrv"
      ${If} $3 != ""
        StrCpy $cfg_idsrv $3
        !insertmacro Log_String "-- idsrv = $cfg_idsrv"
      ${EndIf}
    ${EndIf}
  ${Else}
  !insertmacro Log_String "-- fichier introuvable, création d'un nouveau..."
  ${EndIf}

FunctionEnd

Function SaveConfig
!insertmacro Log_String "[Function SaveConfig]"

  !insertmacro Log_String "Sauvegarde de la configuration."

  nsisXML::create
  nsisXML::load "$INSTDIR\Config\HoMIDom.xml"
  nsisXML::select '/homidom/server'
  ${If} $2 != 0
    nsisXML::setAttribute "ipsoap" $cfg_ipsoap
    nsisXML::setAttribute "portsoap" $cfg_portsoap
    nsisXML::setAttribute "idsrv" $cfg_idsrv
    !insertmacro Log_String "-- ipsoap = $cfg_ipsoap"
    !insertmacro Log_String "-- portsoap = $cfg_portsoap"
    !insertmacro Log_String "-- idsrv = $cfg_idsrv"
  ${EndIf}
  nsisXML::save "$INSTDIR\Config\HoMIDom.xml"
  
  nsisXML::create
  nsisXML::load "$INSTDIR\Config\HoMIAdmiN.xml"
  nsisXML::select '/ArrayOfClServer/ClServer/Adresse'
  ${If} $2 != 0
    nsisXML::setText "$cfg_ipsoap"
  ${EndIf}
  nsisXML::select '/ArrayOfClServer/ClServer/Port'
  ${If} $2 != 0
    nsisXML::setText "$cfg_portsoap"
  ${EndIf}
  nsisXML::select '/ArrayOfClServer/ClServer/Id'
  ${If} $2 != 0
    nsisXML::setText "$cfg_idsrv"
  ${EndIf}
  nsisXML::save "$INSTDIR\Config\HoMIAdmiN.xml"
  
  
  
FunctionEnd

; **********************************************************************
; Success - code exécuté à la fin de l'installation (en cas de success)
; **********************************************************************
Function .onInstSuccess
!insertmacro Log_String "[Function .onInstSuccess]"

  !insertmacro Log_String "Installation terminée avec succès."

FunctionEnd

Function un.onInit
!insertmacro Log_String "[Function un.onInit]"

  ; -- Création du dossier temporaires nécessaires à l'installeur (%TEMP%\ns*****)
  InitPluginsDir

  ; --- déploiement des fichiers nécessaires à l'installation
  File /oname=$PLUGINSDIR\nssm.exe "tools\nssm-2.16\win32\nssm.exe"
  File /oname=$PLUGINSDIR\hitb.exe "tools\hitb-1.0\win32\hitb.exe"

  ${If} ${RunningX64}
    !insertmacro Log_String "Architecture 64-bit detectée."
    SetRegView 64
    StrCpy $platform "x64"
    ReadRegStr $INSTDIR HKLM "SOFTWARE\${PRODUCT_NAME}" "InstallDir"
    !insertmacro Log_String "Détection du dossier d'installation : $INSTDIR"
  ${Else}
    !insertmacro Log_String "Architecture 32-bit detectée."
    SetRegView 32
    StrCpy $platform "x86"
    ReadRegStr $INSTDIR HKLM "SOFTWARE\${PRODUCT_NAME}" "InstallDir"
    !insertmacro Log_String "Détection du dossier d'installation : $INSTDIR"
  ${EndIf}

  StrCpy $HomiServiceName "HoMIServicE"

FunctionEnd

; --- Définition des profils (types) d'installation
InstType "Complete"
InstType "Service uniquement"
InstType "Admin uniquement"
InstType "Client WPF uniquement"

Section "" HoMIDoM_STOPSVC
  !insertmacro Log_String "[HoMIDoM_STOPSVC]"
  SectionIn 1 2
  ; --- Désintallation du service V1 dans tous les cas.
  Call UnInstallHomidomServiceNSSM
  Call KillAllHomidomServices
SectionEnd

Section "" HoMIDoM_FRAMEWORK

  !insertmacro Log_String "[HoMIDoM_FRAMEWORK]"

  ; Fichiers de base : requis par l'ensemble des applications de la solution
  ; Section obligatoire.
  SectionIn RO

  CreateDirectory "$INSTDIR"
  CreateDirectory "$INSTDIR\Bdd"
  CreateDirectory "$INSTDIR\Config"
  CreateDirectory "$INSTDIR\Drivers"
  CreateDirectory "$INSTDIR\Fichiers"
  CreateDirectory "$INSTDIR\Logs"
  CreateDirectory "$INSTDIR\Templates"
  
  SetOutPath "$INSTDIR"
  File "tools\hitb-1.0\win32\hitb.exe"
;  File "HoMIGuI.xml"
  File "..\${PRODUCT_RELEASE}\HoMIDom.DLL"
;  File "..\${PRODUCT_RELEASE}\NCrontab.dll"
    
;  ${If} ${RunningX64}
;    File  /x "*.xml" /x "*.pdb" /x "MjpegProcessor.*" "..\Dll_externes\Homidom-64bits\*"
;  ${Else}
;    File  /x "*.xml" /x "*.pdb" /x "MjpegProcessor.*" "..\Dll_externes\Homidom-32bits\*"
;  ${EndIf}

  SetOutPath "$INSTDIR\Templates"
  File /r "..\${PRODUCT_RELEASE}\Templates\*"

SectionEnd

SectionGroup "HoMIDoM Service" HoMIDoM_SERVICE_GRP

   Section "Service" HoMIDoM_SERVICE

    !insertmacro Log_String "[HoMIDoM_SERVICE]"

    SectionIn 1 2

    SetOverwrite on
    SetOutPath "$INSTDIR"
    ;File "..\${PRODUCT_RELEASE}\HoMIDomService.exe"
    File "..\${PRODUCT_RELEASE}\HoMIService.exe"
;    File "..\${PRODUCT_RELEASE}\HoMIDomWebAPI.dll"
    File "..\${PRODUCT_RELEASE}\*.dll"

    SetOverwrite off
    ;File "..\${PRODUCT_RELEASE}\HoMIDomService.exe.config"
    File "..\${PRODUCT_RELEASE}\HoMIService.exe.config"

    SetOverwrite off
    SetOutPath "$INSTDIR\Bdd"
    File "..\${PRODUCT_RELEASE}\Bdd\homidom.db"
    File "..\${PRODUCT_RELEASE}\Bdd\medias.db"

    SetOverwrite off
    SetOutPath "$INSTDIR\Config"
    File "..\${PRODUCT_RELEASE}\Config\homidom.xml"

    SetOverwrite on
    SetOutPath "$INSTDIR\Drivers"
    File /r "..\${PRODUCT_RELEASE}\Drivers\*.dll"
;    File "..\${PRODUCT_RELEASE}\Drivers\Driver_System.dll"
;    File "..\${PRODUCT_RELEASE}\Drivers\Driver_Virtuel.dll"
;    File "..\${PRODUCT_RELEASE}\Drivers\Driver_Weather.dll"
;    ${If} ${RunningX64}
;      File  "..\Dll_externes\Homidom-64bits\Drivers\Interop.WUApiLib.dll"
;    ${Else}
;      File  "..\Dll_externes\Homidom-32bits\Drivers\Interop.WUApiLib.dll"
;    ${EndIf}

    SetOverwrite on
    SetOutPath "$INSTDIR"
    ${If} ${RunningX64}
    ${Else}
      File /r "..\${PRODUCT_RELEASE}\_DLLx86\*.dll"
    ${EndIf}

  SectionEnd
  
   Section "GUI" HoMIDoM_SVCGUI

    !insertmacro Log_String "[HoMIDoM_SVCGUI]"

    SectionIn 1 2

    SetOverwrite on
    SetOutPath "$INSTDIR"
;    File "HoMIGuI.xml"
    File "..\${PRODUCT_RELEASE}\HoMIGuI.exe"
    File "..\${PRODUCT_RELEASE}\HoMIGuI.exe.config"

    WriteRegStr HKLM "Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\layers" "$INSTDIR\HoMIGuI.exe" "RUNASADMIN"

  SectionEnd
  
;  SectionGroup "Drivers" DRIVERS_GRP
;   !include "includes\Driver_*.nsh"
;  SectionGroupEnd
  
SectionGroupEnd


Section "HoMIDoM Admin" HoMIDoM_ADMIN
  
  !insertmacro Log_String "[HoMIDoM_ADMIN]"
  
  SectionIn 1 3

  SetOverwrite on
  SetOutPath "$INSTDIR"
  File "..\${PRODUCT_RELEASE}\HoMIAdmiN.exe"
  
  SetOverwrite off
  File "..\${PRODUCT_RELEASE}\HoMIAdmiN.exe.Config"
  
  CreateDirectory "$INSTDIR\Images"
  SetOutPath "$INSTDIR\Images"
  File /r "..\${PRODUCT_RELEASE}\Images\*"
  
  SetOverwrite off
  SetOutPath "$INSTDIR\Config"
  File "..\${PRODUCT_RELEASE}\Config\HoMIAdmiN.xml"

SectionEnd

Section "HoMIDoM Client WPF" HoMIDoM_WPFCLIENT

  !insertmacro Log_String "[HoMIDoM_WPF]"

  SectionIn 1 4

  SetOverwrite on
  SetOutPath "$INSTDIR"
  CreateDirectory "$INSTDIR\Config"
  CreateDirectory "$INSTDIR\Logs"
  CreateDirectory "$INSTDIR\Images"
  CreateDirectory "$INSTDIR\Cache\Images"
  ;CreateDirectory "$INSTDIR\HoMIWpf\data"

  SetOverwrite on
  File "..\${PRODUCT_RELEASE}_HoMIWpF\HoMIWpF.*"

  SetOverwrite off
  File "..\${PRODUCT_RELEASE}_HoMIWpF\*.dll"

  SetOverwrite on
  SetOutPath "$INSTDIR\Images"
  File /r "..\${PRODUCT_RELEASE}_HoMIWpF\Images\*"

;  SetOverwrite on
;  SetOutPath "$INSTDIR\HoMIWpf\data"
;  File /r "..\${PRODUCT_RELEASE}_HoMIWpF\data\*"

  SetOverwrite off
  SetOutPath "$INSTDIR\Config"
  File "..\${PRODUCT_RELEASE}_HoMIWpF\Config\*"

SectionEnd

Section "" HoMIDoM_SHORTCUTS

  !insertmacro Log_String "[HoMIDoM_SHORTCUTS]"

  SetOutPath "$INSTDIR"
  CreateDirectory $SMPROGRAMS\${PRODUCT_NAME}
  ; suppression de tous les raccourcis existant
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\*.*"
  
  ; Création du raccourçi pour HomiAdmin
  ${If} ${SectionIsSelected} ${HoMIDoM_ADMIN}
    CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\HoMIDoM Administration.lnk" "$INSTDIR\HoMIAdmin.exe"
  ${EndIf}
  
  ; Création du raccourci vers HomiService mode console
  ${If} ${SectionIsSelected} ${HoMIDoM_SERVICE}
    CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\HoMIDoM Service (Mode Console).lnk" "$INSTDIR\HoMIService.exe"
  ${EndIf}
  
  ${If} ${SectionIsSelected} ${HoMIDoM_SVCGUI}
    CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\HoMIDoM Service Agent (GUI).lnk" "$INSTDIR\HoMIGuI.exe"
  ${EndIf}
  
  ${If} ${SectionIsSelected} ${HoMIDoM_WPFCLIENT}
    CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\HoMIDoM WPF Client.lnk" "$INSTDIR\HoMIWpF.exe"
  ${EndIf}

  ; Remove All old HoMIGuI Autostart
  Delete "$SMSTARTUP\HoMIDoM Service GUI.lnk"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Run\HoMIGuI"
  ExecWait 'SchTasks /Delete /TN HoMIGuI /F'
  
  ${If} $optStartServiceGUI == "1"
    ;SetOutPath "$INSTDIR"

    ; Création de la tache planifiée
    !insertmacro Log_String "Création de la tache planifiée"

    Call Create_HoMIGuI_xml
    ExecWait 'SchTasks /Create /XML $PLUGINSDIR\HoMIGuI.xml /TN HoMIGuI'
    
  ${EndIf}
  
  WriteINIStr "$SMPROGRAMS\${PRODUCT_NAME}\Visitez le site Web HoMIDoM.URL" "InternetShortcut" "URL" "http://www.homidom.com"
  
SectionEnd

Section "" HoMIDoM_POST

  Call WriteUnInstaller
  Call AddFirewallRules
  Call SaveConfig
  Call InstallHomidomService
  Call StartHomidomService

SectionEnd


; **********************************************************************
; Uninstall Section
; **********************************************************************
Function un.ModifyUnWelcome

  ; récupération du dossier %ProgramData%
  System::Call 'shell32::SHGetSpecialFolderPath(i $HWNDPARENT, t .r1, i ${CSIDL_COMMONAPPDATA}, i0)i.r0'
  StrCpy $SF_COMMONAPPDATA $1

  ${NSD_CreateCheckbox} 120u -20u 70% 12u "Supprimer tout : (Base de données, configuration, etc.)"
  Pop $fulluninstbox
  ;SetCtlColors $fulluninstbox "" ${MUI_BGCOLOR}
  ${NSD_UnCheck} $fulluninstbox

FunctionEnd

Function un.LeaveUnWelcome
  ${NSD_GetState} $fulluninstbox $fulluninstbox_state
  ${If} $fulluninstbox_state == ${BST_CHECKED}
    MessageBox mb_yesno "Vous êtes sur le point de supprimer totalement HoMIDoM.$\r$\n$\r$\nSuite à cette opération, une nouvelle installation créera une configuration vièrge de toutes données.$\r$\n$\r$\nVous confirmez ?" IDYES FinishLeaveUnWelcome
    ;${NSD_SetState} $fulluninstbox ${BST_UNCHECKED}
    StrCpy $fulluninstbox_state ${BST_UNCHECKED}
    FinishLeaveUnWelcome:
  ${EndIf}
FunctionEnd

Section "Uninstall"

  SetOutPath "$INSTDIR"

  Banner::show /NOUNLOAD /set 76 "HoMIDoM :" "Désinstallation..."

  ; Lecture du fichier de configuration
  nsisXML::create
  nsisXML::load "$INSTDIR\Config\HoMIDom.xml"
  ; récupération du noeud /homidom/server
  nsisXML::select '/homidom/server'
  ${If} $2 != 0
    nsisXML::getAttribute "portsoap"
    ${If} $3 != ""
      StrCpy $cfg_portsoap $3
    ${EndIf}
  ${EndIf}

  ; Suppression des entrées HoMIDoM dans le parefeu
  SimpleFC::RemoveApplication "$INSTDIR\HoMIServicE.exe"
  Pop $0 ;
  SimpleFC::RemovePort $cfg_portsoap 6
  Pop $0 ;

  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -sps"' "" ; stop-service
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  !insertmacro Log_String "Arret du serveur (hitb -sps) : $0"

  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -ka"' ""  ; kill-all
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  !insertmacro Log_String "Arret du serveur (hitb -ka) : $0"

  ReadRegStr $3 HKLM "${PRODUCT_UNINSTALL_REGKEY}" "ServiceName"

  ; Arret du Service V2
  SimpleSC::ExistsService "$HomiServiceName"
  Pop $0 ; returns an errorcode if the service doesn´t exists (<>0)/service exists (0)
  !insertmacro Log_String "Vérification de l'existance du Service '$HomiServiceName': $3"
  ${If} $0 == 0
    SimpleSC::GetServiceStatus "$HomiServiceName"
    Pop $0 ; returns an errorcode (!=0) otherwise success (0)
    Pop $1 ; return the status of the service (see below)
    !insertmacro Log_String "GetServiceStatus: $0,$1"
    SimpleSC::RemoveService "$HomiServiceName"
    Pop $0 ; returns an errorcode (<>0) otherwise success (0)
    !insertmacro Log_String "Desintallation du service V2: $0"
    ${If} $1 != 1
      !insertmacro Log_String "Arrêt du Service '$HomiServiceName'"
      ; Stop a service and waits for file release. Be sure to pass the service name, not the display name.
      SimpleSC::StopService "$HomiServiceName" 0 30
      SimpleSC::StopService "$HomiServiceName" 1 30
      Pop $0 ; returns an errorcode (<>0) otherwise success (0)
      !insertmacro Log_String "StopService=$0"
      SimpleSC::GetServiceStatus "$HomiServiceName"
      Pop $0 ; returns an errorcode (!=0) otherwise success (0)
      Pop $1 ; return the status of the service (see below)
      !insertmacro Log_String "GetServiceStatus=$0,$1"
    ${EndIf}
  ${EndIf}

  Banner::destroy
  
  Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "Desinstallation en cours..."
  
    ; desinstallation du service existant V1
    ExecCmd::exec /TIMEOUT=5000 '$PLUGINSDIR\nssm.exe remove HoMIDoM confirm'
    Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
    !insertmacro Log_String "Desintallation du service V1: $0"

    Call un.UnInstallHomidomService
    
  Banner::destroy

  ; Remove all HoMIGuI Autostart
  Delete "$SMSTARTUP\HoMIDoM Service GUI.lnk"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Run\HoMIGuI"
  ExecWait 'SchTasks /Delete /TN HoMIGuI /F'
  
  ; Remove registry keys
  DeleteRegKey HKLM "${PRODUCT_UNINSTALL_REGKEY}"

  ${If} $fulluninstbox_State == ${BST_CHECKED}
  !insertmacro RemoveFilesAndSubDirs "$INSTDIR\"
  !insertmacro RemoveFilesAndSubDirs "$SF_COMMONAPPDATA\HoMIAdmiN\"
  !insertmacro RemoveFilesAndSubDirs "$SF_COMMONAPPDATA\HoMIWpF\"
  DeleteRegKey HKLM "SOFTWARE\HoMIDoM"
  DeleteRegKey HKLM "SOFTWARE\HoMIWpF"

  RMDir "$SF_COMMONAPPDATA\HoMIAdmiN"
  RMDir "$SF_COMMONAPPDATA\HoMIWpF"
  RMDir "$INSTDIR"
  
  ${Else}
  Delete $INSTDIR\*
  
  Delete $INSTDIR\Images\*
  RMDir "$INSTDIR\Images"
  
  ;Delete $INSTDIR\Bdd\*
  ;RMDir "$INSTDIR\Bdd"

  ;Delete $INSTDIR\Config\*
  ;RMDir "$INSTDIR\Config"

  ;Delete $INSTDIR\Fichiers\*
  ;RMDir "$INSTDIR\Fichiers"

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

  ; Remove files and uninstaller
  Delete $INSTDIR\uninstall.exe

  ; Remove directories used
  RMDir "$INSTDIR"

  ${EndIf}

  Delete "$SMPROGRAMS\${PRODUCT_NAME}\HoMIDoM Administration.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\HoMIDoM Service GUI.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_NAME}"

SectionEnd

; **********************************************************************
; dé-installation du service windows via NSSM
; **********************************************************************
Function UnInstallHomidomServiceNSSM
!insertmacro Log_String "Function UnInstallHomidomServiceNSSM"
  ${If} $IsHomidomInstalledAsServiceNSSM == "1"
    Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :"  "Désinstallation du service V1."
    ; desinstallation du service existant V1
    ExecCmd::exec /TIMEOUT=5000 '$PLUGINSDIR\nssm.exe remove HoMIDoM confirm'
    Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
    !insertmacro Log_String "Desintallation du service V1: $0"
    Banner::destroy
  ${EndIf}
FunctionEnd

; **********************************************************************
; dé-installation du service windows via InstallUtil
; **********************************************************************
Function un.UnInstallHomidomService
!insertmacro Log_String "Function un.UnInstallHomidomService"
  ${If} $IsHomidomInstalledAsService == "1"
    Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "Désinstallation du service V2."
    ; desinstallation du service existant V2
;    ExecCmd::exec /TIMEOUT=5000 '"$WINDIR\Microsoft.NET\Framework\v4.0.30319\installUtil.Exe" /u "$INSTDIR\HoMIService.exe"'
;    Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
    SimpleSC::RemoveService "$HomiServiceName"
    Pop $0 ; returns an errorcode (<>0) otherwise success (0)
    !insertmacro Log_String "Desintallation du service V2: $0"
    Banner::destroy
  ${EndIf}
FunctionEnd

; **********************************************************************
; installation du service windows via InstallUtil
; **********************************************************************
Function InstallHomidomService
!insertmacro Log_String "Function InstallHomidomService"

${If} $optInstallAsService == "1"

  Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "Installation du Service"
  !insertmacro Log_String "Installation du service via InstallUtil"

  SetOutpath "$INSTDIR"
  ;File "tools\InstallUtil.exe"
  ; installation du service
  ExecWait '"$WINDIR\Microsoft.NET\Framework\v4.0.30319\installUtil.Exe" "$INSTDIR\HoMIService.exe"'

  ; Arret du Service V2
  SimpleSC::ExistsService "$HomiServiceName"
  Pop $0 ; returns an errorcode if the service doesn´t exists (<>0)/service exists (0)
  
  ${If} $0 == "0"
    !insertmacro Log_String "Installation du Service réusise."
  ${Else}
    !insertmacro Log_String "Une erreur est survenue durant l'installation du Service: $0"
  ${EndIf}

  Banner::destroy

${EndIf}

FunctionEnd

; **********************************************************************
; installation & démarrage du service windows
; **********************************************************************
Function StartHomidomService
!insertmacro Log_String "Function StartHomidomService"

  Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "Démarrage du service..."
  !insertmacro Log_String "Démarrage du service : $HomiServiceName"

  ; --- Démarrage du serveur (si nécessaire)
  ${If} $optStartService == "1"
    ${If} $optInstallAsService == "1"
      SimpleSC::SetServiceStartType "$HomiServiceName" "2"
      Pop $0 ; returns an errorcode (<>0) otherwise success (0)
      ; Start a service. Be sure to pass the service name, not the display name.
      SimpleSC::StartService "$HomiServiceName" "" 30
      Pop $0 ; returns an errorcode (<>0) otherwise success (0)

    ${Else}
      SetOutPath "$INSTDIR"
      Exec '"$INSTDIR\HoMIServicE.exe"'
    ${EndIf}
  ${EndIf}
  ${If} $optStartServiceGUI == "1"
      SetOutPath "$INSTDIR"
      Exec '"$INSTDIR\HomiGUI.exe"'
;      NotifyIcon::Icon "yf" "HomiGUI.exe"

  ${EndIf}

  Banner::destroy

FunctionEnd

Function AddFirewallRules
!insertmacro Log_String "Function AddFirewallRules"

  ${If} $optInstallAsService == "1"
  
    ; Check if the port 7999/TCP is enabled/disabled
    ;SimpleFC::IsPortEnabled $cfg_portsoap 6
    
    SimpleFC::IsApplicationAdded "$INSTDIR\HoMIServicE.exe"
    Pop $0 ; return error(1)/success(0)
    Pop $1 ; return 1=Enabled/0=Not enabled
    ${If} $0 == 0
      ${If} $1 == 0
        Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "Configuration du pare-feu..."
        
        ; Add the port xxxxx/TCP to the firewall exception list - All Networks - All IP Version - Enabled
        ;SimpleFC::AddPort [port] [name] [protocol] [scope] [ip_version] [remote_addresses] [status]
        SimpleFC::AddPort $cfg_portsoap "HoMIDoM Service" 6 0 2 "" 1

        ; Add an application to the firewall exception list - All Networks - All IP Version - Enabled
        ;SimpleFC::AddApplication [name] [path] [scope] [ip_version] [remote_addresses] [status]
        SimpleFC::AddApplication "HoMIDoM Service" "$INSTDIR\HoMIServicE.exe" 0 2 "" 1
        Pop $0 ; return error(1)/success(0)
        
        !insertmacro Log_String "Configuration du pare-feu. Ouverture du Port TCP $cfg_portsoap : $0"
        !insertmacro Log_String "Configuration du pare-feu. Ajout de l'application $INSTDIR\HoMIServicE.exe : $0"
        Banner::destroy
        
      ${Else}
        !insertmacro Log_String "Le pare-feu est déja configuré correctement."
      ${EndIf}


    ${EndIf}
  ${EndIf}

FunctionEnd

; **********************************************************************
; Arrêt de toutes les application & services HoMIDom
; **********************************************************************
Function KillAllHomidomServices
!insertmacro Log_String "Function KillAllHomidomServices"

  !insertmacro Log_String "[KillAllHomidomServices]"

  Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "Arrêt d'HoMIDoM..."
  
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -sps"' "" ; stop-service
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  !insertmacro Log_String "Arret du serveur (hitb -sps) : $0"
  
  ExecCmd::exec /TIMEOUT=5000 '"$PLUGINSDIR\hitb.exe -ka"' ""  ; kill-all
  Pop $0 ; return value - process exit code or error or STILL_ACTIVE (0x103).
  !insertmacro Log_String "Arret du serveur (hitb -ka) : $0"

  ; Arret du Service V2
  SimpleSC::ExistsService "$HomiServiceName"
  Pop $0 ; returns an errorcode if the service doesn´t exists (<>0)/service exists (0)
  !insertmacro Log_String "Vérification de l'existance du Service: $0"
  ${If} $0 == 0
    SimpleSC::GetServiceStatus "$HomiServiceName"
    Pop $0 ; returns an errorcode (!=0) otherwise success (0)
    Pop $1 ; return the status of the service (see below)
    !insertmacro Log_String "GetServiceStatus: $0,$1"
    SimpleSC::RemoveService "$HomiServiceName"
    Pop $0 ; returns an errorcode (<>0) otherwise success (0)
    !insertmacro Log_String "Desintallation du service V2: $0"
    ${If} $1 != 1
      !insertmacro Log_String "Arrêt du Service '$HomiServiceName'"
      ; Stop a service and waits for file release. Be sure to pass the service name, not the display name.
      SimpleSC::StopService "$HomiServiceName" 0 30
      SimpleSC::StopService "$HomiServiceName" 1 30
      Pop $0 ; returns an errorcode (<>0) otherwise success (0)
      !insertmacro Log_String "StopService=$0"
      SimpleSC::GetServiceStatus "$R0"
      Pop $0 ; returns an errorcode (!=0) otherwise success (0)
      Pop $1 ; return the status of the service (see below)
      !insertmacro Log_String "GetServiceStatus=$0,$1"
    ${EndIf}
  ${EndIf}
  
  Banner::destroy
  
FunctionEnd

Function nsDialogsPage
!insertmacro Log_String "Function nsDialogsPage"
!insertmacro MUI_HEADER_TEXT "Options d'installation" "Configurer les options"

  nsDialogs::Create 1018
  Pop $Dialog

  ${If} $Dialog == error
    Abort
  ${EndIf}

  strcpy $IsComposantChecked "False"
  ${If} ${SectionIsSelected} ${HoMIDoM_SERVICE}
    strcpy $IsComposantChecked "True"
  ${Endif}
  ${If} ${SectionIsSelected} ${HoMIDoM_SVCGUI}
    strcpy $IsComposantChecked "True"
  ${Endif}
  ${If} ${SectionIsSelected} ${HoMIDoM_ADMIN}
    strcpy $IsComposantChecked "True"
  ${Endif}
  ${If} ${SectionIsSelected} ${HoMIDoM_WPFCLIENT}
    strcpy $IsComposantChecked "True"
  ${Endif}
  ${If} $IsComposantChecked == "True"
    ;MessageBox MB_ICONSTOP|MB_OK "Au moins un composant a été sélectionné."
    #1 - Next, 2 - Cancel, 3 - Back
    GetDlgItem $0 $HWNDPARENT 1 #1 for 'Next' button
    EnableWindow $0 1
    ;Call EnableNextButton
  ${Else}
    MessageBox MB_ICONSTOP|MB_OK "ERREUR : Aucun composant HoMIDoM n'est séléctionné !$\nLa suite de l'installation est impossible.$\n$\nVeuillez selectionner au moins un composant."
    #1 - Next, 2 - Cancel, 3 - Back
    GetDlgItem $0 $HWNDPARENT 1 #1 for 'Next' button
    EnableWindow $0 0
    ;Call DisableNextButton
  ${Endif}
  StrCpy $optInstallAsService "1"
  StrCpy $optCreateStartMenuShortcuts "1"
  StrCpy $optStartService "1"
  StrCpy $optStartServiceGUI "1"

  ; CreateStartMenuShortcuts
  ${NSD_CreateCheckbox} 0 0u 100% 10u "Créer les raccourçis dans le menu Démarrer"
  Pop $chkCreateStartMenuShortcuts_Handle
  ${If} $optCreateStartMenuShortcuts == ${BST_CHECKED}
    ${NSD_Check} $chkCreateStartMenuShortcuts_Handle
  ${EndIf}

  ${If} ${SectionIsSelected} ${HoMIDoM_SERVICE}

    ${NSD_CreateHLine} 0 15u 100% 0u ""
    ; ---------------------------------------------------------------
    ${NSD_CreateCheckbox} 0 20u 100% 10u "Installer HoMIDoM Service en tant que Service Windows"
    Pop $chkInstallAsService_Handle
    ${If} $optInstallAsService == ${BST_CHECKED}
      ${NSD_Check} $chkInstallAsService_Handle
    ${EndIf}

    ${NSD_CreateCheckbox} 0 35u 100% 10u "Demarrer le Service HoMIDoM à la fin de l'installation"
    Pop $chkStartService_Handle
    ${If} $optStartService == ${BST_CHECKED}
      ${NSD_Check} $chkStartService_Handle
    ${EndIf}
    ; ---------------------------------------------------------------
    ${NSD_CreateHLine} 0 50u 100% 0u ""

    ${NSD_CreateLabel} 10u 55u 50u 10u "IP/Nom d'hôte : "
    ${NSD_CreateText} 75u 55u 30% 10u $cfg_ipsoap
    Pop $txtCfgIpSoap_Handle
    
    ${NSD_CreateLabel} 10u 70u 50u 10u "Port : "
    ${NSD_CreateText} 75u 70u 30% 10u $cfg_portsoap
    Pop $txtCfgPortSoap_Handle
    
    ${NSD_CreateLabel} 10u 85u 50u 10u "ID serveur : "
    ${NSD_CreateText} 75u 85u 30% 10u $cfg_idsrv
    Pop $txtCfgIdSrv_Handle

    ${NSD_CreateCheckbox} 0 110u 100% 10u "Demarrer automatiquement HoMIDoM GUI au démarrage du PC"
    Pop $chkStartServiceGUI_Handle
    ${If} $optStartServiceGUI == ${BST_CHECKED}
      ${NSD_Check} $chkStartServiceGUI_Handle
    ${EndIf}

  ${EndIf}
  nsDialogs::Show
  
FunctionEnd

Function nsDialogsPageLeave
!insertmacro Log_String "Function nsDialogsPageLeave"
         ${NSD_GetState} $chkInstallAsService_Handle $optInstallAsService
         ${NSD_GetState} $chkStartService_Handle $optStartService
         ${NSD_GetState} $chkCreateStartMenuShortcuts_Handle $optCreateStartMenuShortcuts
         ${NSD_GetState} $chkStartServiceGUI_Handle $optStartServiceGUI
         
         ${NSD_GetText} $txtCfgIpSoap_Handle $cfg_ipsoap
         !insertmacro Log_String "IPSOAP=$cfg_ipsoap"
         ${NSD_GetText} $txtCfgPortSoap_Handle $cfg_portsoap
         !insertmacro Log_String "PORTSOAP=$cfg_portsoap"
         ${NSD_GetText} $txtCfgIdSrv_Handle $cfg_idsrv
         !insertmacro Log_String "IDSRV=$cfg_idsrv"
FunctionEnd

Function WriteUnInstaller
!insertmacro Log_String "[Function WriteUnInstaller]"

  Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "Mise à jour de la base de registre."

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
  WriteRegStr HKLM "${PRODUCT_UNINSTALL_REGKEY}" "ServiceName" "$HomiServiceName"
  WriteUninstaller "uninstall.exe"
  
  Banner::destroy
  
FunctionEnd

Function Create_HoMIGuI_xml

FileOpen $0 $PLUGINSDIR\HoMIGuI.xml w
FileWrite $0 '<?xml version="1.0" encoding="UTF-16"?>$\r$\n'
FileWrite $0 '<Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">$\r$\n'
FileWrite $0 '  <RegistrationInfo>$\r$\n'
FileWrite $0 '    <Date>2016-07-07T21:00:13.8007254</Date>$\r$\n'
FileWrite $0 '    <Author>HoMIDoM</Author>$\r$\n'
FileWrite $0 '    <URI>\HoMIGuI</URI>$\r$\n'
FileWrite $0 '  </RegistrationInfo>$\r$\n'
FileWrite $0 '  <Triggers>$\r$\n'
FileWrite $0 '    <LogonTrigger id="TriggerUserLoggon">$\r$\n'
FileWrite $0 '      <Enabled>true</Enabled>$\r$\n'
FileWrite $0 '      <Delay>PT30S</Delay>$\r$\n'
FileWrite $0 '    </LogonTrigger>$\r$\n'
FileWrite $0 '  </Triggers>$\r$\n'
FileWrite $0 '  <Principals>$\r$\n'
FileWrite $0 '    <Principal id="Author">$\r$\n'
FileWrite $0 '      <GroupId>S-1-5-4</GroupId>$\r$\n'
FileWrite $0 '      <RunLevel>HighestAvailable</RunLevel>$\r$\n'
FileWrite $0 '    </Principal>$\r$\n'
FileWrite $0 '  </Principals>$\r$\n'
FileWrite $0 '  <Settings>$\r$\n'
FileWrite $0 '    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>$\r$\n'
FileWrite $0 '    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>$\r$\n'
FileWrite $0 '    <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>$\r$\n'
FileWrite $0 '    <AllowHardTerminate>true</AllowHardTerminate>$\r$\n'
FileWrite $0 '    <StartWhenAvailable>true</StartWhenAvailable>$\r$\n'
FileWrite $0 '    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>$\r$\n'
FileWrite $0 '    <IdleSettings>$\r$\n'
FileWrite $0 '      <StopOnIdleEnd>true</StopOnIdleEnd>$\r$\n'
FileWrite $0 '      <RestartOnIdle>false</RestartOnIdle>$\r$\n'
FileWrite $0 '    </IdleSettings>$\r$\n'
FileWrite $0 '    <AllowStartOnDemand>true</AllowStartOnDemand>$\r$\n'
FileWrite $0 '    <Enabled>true</Enabled>$\r$\n'
FileWrite $0 '    <Hidden>false</Hidden>$\r$\n'
FileWrite $0 '    <RunOnlyIfIdle>false</RunOnlyIfIdle>$\r$\n'
FileWrite $0 '    <WakeToRun>false</WakeToRun>$\r$\n'
FileWrite $0 '    <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>$\r$\n'
FileWrite $0 '    <Priority>7</Priority>$\r$\n'
FileWrite $0 '  </Settings>$\r$\n'
FileWrite $0 '  <Actions Context="Author">$\r$\n'
FileWrite $0 '    <Exec>$\r$\n'
FileWrite $0 '      <Command>"$INSTDIR\HoMIGuI.exe"</Command>$\r$\n'
FileWrite $0 '    </Exec>$\r$\n'
FileWrite $0 '  </Actions>$\r$\n'
FileWrite $0 '</Task>$\r$\n'
FileClose $0

FunctionEnd
