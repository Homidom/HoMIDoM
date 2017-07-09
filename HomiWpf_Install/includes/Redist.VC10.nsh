!define URL_VC10_X64 "http://download.microsoft.com/download/3/2/2/3224B87F-CFA0-4E70-BDA3-3DE650EFEBA5/vcredist_x64.exe"
!define URL_VC10_X86 "http://download.microsoft.com/download/5/B/C/5BC5DBB3-652D-4DCE-B14A-475AB85EEF6E/vcredist_x86.exe"

; Microsoft Visual C++ 2010 (10.0) Redistributable Package 

var URL_VC10
var VC10_Package
Var VC10_RegKey

Var VC10RedistRegKeyValue
Var IsVC10RedistInstalled

Function DownloadVC10Redist
 

      ; the following Goto and Label is for consistencey.
      Goto lbl_DownloadRequired
      lbl_DownloadRequired:
      DetailPrint "$(DESC_DOWNLOADING1) $(DESC_VC10_LABEL)..."
      MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "$(DESC_VC10_DECISION)" /SD IDNO \
        IDYES lbl_Download IDNO 0
      !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable : Installation réfusée."
      Goto lbl_NoDownloadRequired ;Abort

      lbl_Download:
      nsisdl::download /TRANSLATE "$(DESC_DOWNLOADING)" "$(DESC_CONNECTING)" \
         "$(DESC_SECOND)" "$(DESC_MINUTE)" "$(DESC_HOUR)" "$(DESC_PLURAL)" \
         "$(DESC_PROGRESS)" "$(DESC_REMAINING)" \
         /TIMEOUT=30000 "$URL_VC10" "$PLUGINSDIR\$VC10_Package"
      Pop $0
      StrCmp "$0" "success" lbl_continue
      DetailPrint "$(DESC_DOWNLOADFAILED) $0"
      Goto lbl_NoDownloadRequired
      ;Abort

      lbl_continue:
        !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable : Téléchargement OK."
        DetailPrint "$(DESC_INSTALLING) $(DESC_VC10_LABEL)..."
        Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "$(DESC_INSTALLING) $(DESC_VC10_LABEL)..."
        nsExec::ExecToStack '"$PLUGINSDIR\$VC10_Package" /q :a'
        ;pop $DOTNET_RETURN_CODE
        Banner::destroy
        SetRebootFlag true
        ; silence the compiler
        Goto lbl_NoDownloadRequired
        lbl_NoDownloadRequired:

FunctionEnd

Function CheckVC10Redist

  StrCpy $IsVC10RedistInstalled "0"

  ${If} ${RunningX64}
    StrCpy $URL_VC10 "${URL_VC10_X64}"
    StrCpy $VC10_Package "vcredist_x64.exe"
  ${Else}
    StrCpy $URL_VC10 "${URL_VC10_X86}"
    StrCpy $VC10_Package "vcredist_x86.exe"
  ${EndIf}

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing\10.0\RuntimeMinimum"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Install"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Microsoft\DevDiv\vc\Servicing\10.0\RuntimeMinimum"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Install"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing\10.0\red"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Install"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\10.0\VC\Runtimes\x64"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\10.0\VC\VCRedist\x64"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Microsoft\VisualStudio\10.0\VC\Runtimes\x64"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\x64"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\10.0\VC\Runtimes\x86"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\10.0\VC\VCRedist\x86"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Microsoft\VisualStudio\10.0\VC\Runtimes\x86"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ClearErrors
  StrCpy $VC10_RegKey "SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\x86"
  ReadRegDWORD $VC10RedistRegKeyValue HKLM "$VC10_RegKey" "Installed"
  IntCmp $VC10RedistRegKeyValue 1 VC10RegKeyFound

  ;${If} ${FileExists} "$SYSDIR\msvcr100.dll"
  ;  StrCpy $IsVC10RedistInstalled "1"
  ;${EndIf}

  ;!insertmacro Log_String "$(DESC_VC10_LABEL) Redistributable KO :  Pas de clé de registre valide trouvé ! vérification de la présence de msvcr100.dll : $IsVC10RedistInstalled"
  !insertmacro Log_String "$(DESC_VC10_LABEL) Redistributable KO :  Pas de clé de registre valide trouvé !"
  ;call DownloadVC10Redist
  Goto exitVC10RedistCheck

VC10RegKeyFound:

  !insertmacro Log_String "$(DESC_VC10_LABEL) Redistributable OK : $VC10_RegKey => $VC10RedistRegKeyValue"
  StrCpy $IsVC10RedistInstalled "1"

exitVC10RedistCheck:

FunctionEnd

  Section "" PREREQ_VC10
    SectionIn RO
    ${if} $IsVC10RedistInstalled == "0"
      call DownloadVC10Redist
    ${EndIf}
  SectionEnd