
!define URL_VC2010_X64 "http://download.microsoft.com/download/3/2/2/3224B87F-CFA0-4E70-BDA3-3DE650EFEBA5/vcredist_x64.exe"
!define URL_VC2010_X86 "http://download.microsoft.com/download/5/B/C/5BC5DBB3-652D-4DCE-B14A-475AB85EEF6E/vcredist_x86.exe"

; Microsoft Visual C++ 2010 Redistributable Package 

var URL_VC2010
var VC2010_Package
Var VC2010_RegKey

Var VC2010RedistRegKeyValue
Var IsVC2010RedistInstalled

Function DownloadVC2010Redist

  ${If} ${RunningX64}
    StrCpy $URL_VC2010 "${URL_VC2010_X64}"
    StrCpy $VC2010_Package "vcredist_x64.exe"
    StrCpy $VC2010_RegKey "SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\x64"
  ${Else}
    StrCpy $URL_VC2010 "${URL_VC2010_X86}"
    StrCpy $VC2010_Package "vcredist_x86.exe"
   StrCpy $VC2010_RegKey "SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\x86"    
  ${EndIf}

  

      ; the following Goto and Label is for consistencey.
      Goto lbl_DownloadRequired
      lbl_DownloadRequired:
      DetailPrint "$(DESC_DOWNLOADING1) $(DESC_VC2010_LABEL)..."
      MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "$(DESC_VC2010_DECISION)" /SD IDNO \
        IDYES +2 IDNO 0
      Abort

      nsisdl::download /TRANSLATE "$(DESC_DOWNLOADING)" "$(DESC_CONNECTING)" \
         "$(DESC_SECOND)" "$(DESC_MINUTE)" "$(DESC_HOUR)" "$(DESC_PLURAL)" \
         "$(DESC_PROGRESS)" "$(DESC_REMAINING)" \
         /TIMEOUT=30000 "$URL_VC2010" "$PLUGINSDIR\$VC2010_Package"
      Pop $0
      StrCmp "$0" "success" lbl_continue
      DetailPrint "$(DESC_DOWNLOADFAILED) $0"
      Abort

      lbl_continue:
        DetailPrint "$(DESC_INSTALLING) $(DESC_VC2010_LABEL)..."
        Banner::show /NOUNLOAD "$(DESC_INSTALLING) $(DESC_VC2010_LABEL)..."
        nsExec::ExecToStack '"$PLUGINSDIR\$VC2010_Package" /q :a'
        pop $DOTNET_RETURN_CODE
        Banner::destroy
        SetRebootFlag true
        ; silence the compiler
        Goto lbl_NoDownloadRequired
        lbl_NoDownloadRequired:

FunctionEnd

Function CheckVC2010Redist

         ; Registry: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\x86

         ClearErrors
         ; Read  from Registry
         ReadRegDWORD $VC2010RedistRegKeyValue HKLM "$VC2010_RegKey" "Installed"
         IfErrors noVC2010Redist

         StrCpy $IsVC2010RedistInstalled "$VC2010RedistRegKeyValue"
         Goto exitVC2010RedistCheck

noVC2010Redist:
         StrCpy $IsVC2010RedistInstalled "0"
         !insertmacro Log_String "Microsoft Visual C++ 2010 Redistributable  introuvable !"

exitVC2010RedistCheck:

FunctionEnd

  Section "" PREREQ_VC2010
    SectionIn RO
    ${if} $IsVC2010RedistInstalled == "0"
      call DownloadVC2010Redist
    ${EndIf}
  SectionEnd