!define URL_VC12_X64 "https://download.microsoft.com/download/A/4/D/A4D9F1D3-6449-49EB-9A6E-902F61D8D14B/vcredist_x64.exe"
!define URL_VC12_X86 "https://download.microsoft.com/download/A/4/D/A4D9F1D3-6449-49EB-9A6E-902F61D8D14B/vcredist_x86.exe"

; Microsoft Visual C++ 2013 (12.0) Redistributable Package 

var URL_VC12
var VC12_Package
Var VC12_RegKey

Var VC12RedistRegKeyValue
Var IsVC12RedistInstalled

Function DownloadVC12Redist
 
      ; the following Goto and Label is for consistencey.
      Goto lbl_DownloadRequired
      lbl_DownloadRequired:
      DetailPrint "$(DESC_DOWNLOADING1) $(DESC_VC12_LABEL)..."
      MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "$(DESC_VC12_DECISION)" /SD IDNO \
        IDYES lbl_Download IDNO 0
      !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable : Installation réfusée."
      Goto lbl_NoDownloadRequired ;Abort

      lbl_Download:
      nsisdl::download /TRANSLATE "$(DESC_DOWNLOADING)" "$(DESC_CONNECTING)" \
         "$(DESC_SECOND)" "$(DESC_MINUTE)" "$(DESC_HOUR)" "$(DESC_PLURAL)" \
         "$(DESC_PROGRESS)" "$(DESC_REMAINING)" \
         /TIMEOUT=30000 "$URL_VC12" "$PLUGINSDIR\$VC12_Package"
      Pop $0
      StrCmp "$0" "success" lbl_continue
      DetailPrint "$(DESC_DOWNLOADFAILED) $0"
      Goto lbl_NoDownloadRequired ;Abort

      lbl_continue:
        !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable : Téléchargement OK."
        DetailPrint "$(DESC_INSTALLING) $(DESC_VC12_LABEL)..."
        Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "$(DESC_INSTALLING) $(DESC_VC12_LABEL)..."
        nsExec::ExecToStack '"$PLUGINSDIR\$VC12_Package" /q :a'
        ;pop $DOTNET_RETURN_CODE
        Banner::destroy
        SetRebootFlag true
        ; silence the compiler
        Goto lbl_NoDownloadRequired
        lbl_NoDownloadRequired:

FunctionEnd

Function CheckVC12Redist

  StrCpy $IsVC12RedistInstalled "0"

  ${If} ${RunningX64}
    StrCpy $URL_VC12 "${URL_VC12_X64}"
    StrCpy $VC12_Package "vcredist_x64.exe"
  ${Else}
    StrCpy $URL_VC12 "${URL_VC12_X86}"
    StrCpy $VC12_Package "vcredist_x86.exe"
  ${EndIf}

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing\12.0\RuntimeMinimum"
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Install"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Microsoft\DevDiv\vc\Servicing\12.0\RuntimeMinimum"
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Install"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing\12.0\red"
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Install"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Microsoft\DevDiv\vc\Servicing\12.0\red" 
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Install"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\VC\Runtimes\x64"
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Installed"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Microsoft\VisualStudio\12.0\VC\Runtimes\x64"
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Installed"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\VC\Runtimes\x86"
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Installed"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ClearErrors
  StrCpy $VC12_RegKey "SOFTWARE\Microsoft\VisualStudio\12.0\VC\Runtimes\x86"
  ReadRegDWORD $VC12RedistRegKeyValue HKLM "$VC12_RegKey" "Installed"
  IntCmp $VC12RedistRegKeyValue 1 VC12RegKeyFound

  ;${If} ${FileExists} "$SYSDIR\msvcr120.dll"
  ;  StrCpy $IsVC12RedistInstalled "1"
  ;${EndIf}

  ;!insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable KO :  Pas de clé de registre valide trouvée ! vérification de la présence de msvcr120.dll : $IsVC12RedistInstalled"
  !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable KO :  Pas de clé de registre valide trouvée !"
  ;call DownloadVC12Redist
  Goto exitVC12RedistCheck

VC12RegKeyFound:

  !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable OK : $VC12_RegKey => $VC12RedistRegKeyValue"
  StrCpy $IsVC12RedistInstalled "1"

exitVC12RedistCheck:

FunctionEnd

  Section "" PREREQ_VC12
    SectionIn RO
    ${if} $IsVC12RedistInstalled == "0"
      call DownloadVC12Redist
    ${EndIf}
  SectionEnd