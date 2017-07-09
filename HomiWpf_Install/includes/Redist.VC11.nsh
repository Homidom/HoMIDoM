!define URL_VC11_X64 "http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU3/vcredist_x64.exe"
!define URL_VC11_X86 "http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU3/vcredist_x86.exe"

; Microsoft Visual C++ 2012 (11.0) Redistributable Package 

var URL_VC11
var VC11_Package
Var VC11_RegKey

Var VC11RedistRegKeyValue
Var IsVC11RedistInstalled

Function DownloadVC11Redist
 
      ; the following Goto and Label is for consistencey.
      Goto lbl_DownloadRequired
      lbl_DownloadRequired:
      DetailPrint "$(DESC_DOWNLOADING1) $(DESC_VC11_LABEL)..."
      MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "$(DESC_VC11_DECISION)" /SD IDNO \
        IDYES lbl_Download IDNO 0
      !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable : Installation réfusée."
      Goto lbl_NoDownloadRequired ;Abort

      lbl_Download:
      nsisdl::download /TRANSLATE "$(DESC_DOWNLOADING)" "$(DESC_CONNECTING)" \
         "$(DESC_SECOND)" "$(DESC_MINUTE)" "$(DESC_HOUR)" "$(DESC_PLURAL)" \
         "$(DESC_PROGRESS)" "$(DESC_REMAINING)" \
         /TIMEOUT=30000 "$URL_VC11" "$PLUGINSDIR\$VC11_Package"
      Pop $0
      StrCmp "$0" "success" lbl_continue
      DetailPrint "$(DESC_DOWNLOADFAILED) $0"
      Goto lbl_NoDownloadRequired ;Abort

      lbl_continue:
        !insertmacro Log_String "$(DESC_VC12_LABEL) Redistributable : Téléchargement OK."
        DetailPrint "$(DESC_INSTALLING) $(DESC_VC11_LABEL)..."
        Banner::show /NOUNLOAD /set 76 "Installation d'HoMIDoM :" "$(DESC_INSTALLING) $(DESC_VC11_LABEL)..."
        nsExec::ExecToStack '"$PLUGINSDIR\$VC11_Package" /q :a'
        ;pop $DOTNET_RETURN_CODE
        Banner::destroy
        SetRebootFlag true
        ; silence the compiler
        Goto lbl_NoDownloadRequired
        lbl_NoDownloadRequired:

FunctionEnd

Function CheckVC11Redist

  StrCpy $IsVC11RedistInstalled "0"

  ${If} ${RunningX64}
    StrCpy $URL_VC11 "${URL_VC11_X64}"
    StrCpy $VC11_Package "vcredist_x64.exe"
  ${Else}
    StrCpy $URL_VC11 "${URL_VC11_X86}"
    StrCpy $VC11_Package "vcredist_x86.exe"
  ${EndIf}

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing\11.0\RuntimeMinimum"
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Install"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Microsoft\DevDiv\vc\Servicing\11.0\RuntimeMinimum"
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Install"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing\11.0\red"
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Install"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Microsoft\DevDiv\vc\Servicing\11.0\red" 
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Install"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\11.0\VC\Runtimes\x64"
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Installed"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Microsoft\VisualStudio\11.0\VC\Runtimes\x64"
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Installed"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Wow6432Node\Microsoft\VisualStudio\11.0\VC\Runtimes\x86"
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Installed"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ClearErrors
  StrCpy $VC11_RegKey "SOFTWARE\Microsoft\VisualStudio\11.0\VC\Runtimes\x86"
  ReadRegDWORD $VC11RedistRegKeyValue HKLM "$VC11_RegKey" "Installed"
  IntCmp $VC11RedistRegKeyValue 1 VC11RegKeyFound

  ;${If} ${FileExists} "$SYSDIR\msvcr110.dll"
  ;  StrCpy $IsVC11RedistInstalled "1"
  ;${EndIf}

  ;!insertmacro Log_String "$(DESC_VC11_LABEL) Redistributable KO :  Pas de clé de registre valide trouvé ! vérification de la présence de msvcr100.dll : $IsVC11RedistInstalled"
  !insertmacro Log_String "$(DESC_VC11_LABEL) Redistributable KO :  Pas de clé de registre valide trouvé !"
  ;call DownloadVC11Redist
  Goto exitVC11RedistCheck

VC11RegKeyFound:

  !insertmacro Log_String "$(DESC_VC11_LABEL) Redistributable OK : $VC11_RegKey => $VC11RedistRegKeyValue"
  StrCpy $IsVC11RedistInstalled "1"

exitVC11RedistCheck:

FunctionEnd

  Section "" PREREQ_VC11
    SectionIn RO
    ${if} $IsVC11RedistInstalled == "0"
      call DownloadVC11Redist
    ${EndIf}
  SectionEnd