; Usage
; Define in your script two constants:
;   DOT_MAJOR "(Major framework version)"
;   DOT_MINOR "{Minor framework version)"
;
; Call IsDotNetInstalled
; This function will abort the installation if the required version
; or higher version of the .NET Framework is not installed.  Place it in
; either your .onInit function or your first install section before
; other code.

var IsDotNetInstalled


Function CheckDotNetInstalled

  StrCpy $0 "0"
  StrCpy $1 "SOFTWARE\Microsoft\.NETFramework" ;registry entry to look in.
  StrCpy $2 0

  StartEnum:
    ;Enumerate the versions installed.
    EnumRegKey $3 HKLM "$1\policy" $2

    ;If we don't find any versions installed, it's not here.
    StrCmp $3 "" noDotNet notEmpty

    ;We found something.
    notEmpty:
      ;Find out if the RegKey starts with 'v'.
      ;If it doesn't, goto the next key.
      StrCpy $4 $3 1 0
      StrCmp $4 "v" +1 goNext
      StrCpy $4 $3 1 1

      ;It starts with 'v'.  Now check to see how the installed major version
      ;relates to our required major version.
      ;If it's equal check the minor version, if it's greater,
      ;we found a good RegKey.
      IntCmp $4 ${DOT_MAJOR} +1 goNext yesDotNetReg
      ;Check the minor version.  If it's equal or greater to our requested
      ;version then we're good.
      StrCpy $4 $3 1 3
      IntCmp $4 ${DOT_MINOR} yesDotNetReg goNext yesDotNetReg

    goNext:
      ;Go to the next RegKey.
      IntOp $2 $2 + 1
      goto StartEnum

  yesDotNetReg:
    ;Now that we've found a good RegKey, let's make sure it's actually
    ;installed by getting the install path and checking to see if the
    ;mscorlib.dll exists.
    EnumRegValue $2 HKLM "$1\policy\$3" 0
    ;$2 should equal whatever comes after the major and minor versions
    ;(ie, v1.1.4322)
    StrCmp $2 "" noDotNet
    ReadRegStr $4 HKLM $1 "InstallRoot"
    ;Hopefully the install root isn't empty.
    StrCmp $4 "" noDotNet
    ;build the actuall directory path to mscorlib.dll.
    StrCpy $4 "$4$3.$2\mscorlib.dll"
    IfFileExists $4 yesDotNet noDotNet

  noDotNet:
    ;Nope, something went wrong along the way.  Looks like the
    ;proper .NET Framework isn't installed.
    ; MessageBox MB_OK "Le .NET Framework v${DOT_MAJOR}.${DOT_MINOR} ou supérieur doit être installé."
    StrCpy $IsDotNetInstalled "0"
    ;Abort
    goto exitDotNetCheck
    
  yesDotNet:
    ;Everything checks out.  Go on with the rest of the installation.
    StrCpy $IsDotNetInstalled "1"

  exitDotNetCheck:
FunctionEnd


Var "DOTNET_RETURN_CODE"

  Section "" PREREQ_DOTNETFX

      SectionIn RO

      strcmp $IsDotNetInstalled "1" lbl_IsSilent

      ; the following Goto and Label is for consistencey.
      Goto lbl_DownloadRequired
      lbl_DownloadRequired:
      DetailPrint "$(DESC_DOWNLOADING1) $(DESC_SHORTDOTNET)..."
      MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "$(DESC_DOTNET_DECISION)" /SD IDNO \
        IDYES +2 IDNO 0
      Abort

      ; "Downloading Microsoft .Net Framework"
      AddSize 48100
      nsisdl::download /TRANSLATE "$(DESC_DOWNLOADING)" "$(DESC_CONNECTING)" \
         "$(DESC_SECOND)" "$(DESC_MINUTE)" "$(DESC_HOUR)" "$(DESC_PLURAL)" \
         "$(DESC_PROGRESS)" "$(DESC_REMAINING)" \
         /TIMEOUT=30000 "${URL_DOTNET}" "$PLUGINSDIR\dotnetfx.exe"
      Pop $0
      StrCmp "$0" "success" lbl_continue
      DetailPrint "$(DESC_DOWNLOADFAILED) $0"
      Abort

      lbl_continue:
        DetailPrint "$(DESC_INSTALLING) $(DESC_SHORTDOTNET)..."
        Banner::show /NOUNLOAD "$(DESC_INSTALLING) $(DESC_SHORTDOTNET)..."
        nsExec::ExecToStack '"$PLUGINSDIR\dotnetfx.exe" /q /c:"install.exe /noaspupgrade /q"'
        pop $DOTNET_RETURN_CODE
        Banner::destroy
        SetRebootFlag true
        ; silence the compiler
        Goto lbl_NoDownloadRequired
        lbl_NoDownloadRequired:

        ; obtain any error code and inform the user ($DOTNET_RETURN_CODE)
        ; If nsExec is unable to execute the process,
        ; it will return "error"
        ; If the process timed out it will return "timeout"
        ; else it will return the return code from the executed process.
        StrCmp "$DOTNET_RETURN_CODE" "" lbl_NoError
        StrCmp "$DOTNET_RETURN_CODE" "0" lbl_NoError
        StrCmp "$DOTNET_RETURN_CODE" "3010" lbl_NoError
        StrCmp "$DOTNET_RETURN_CODE" "8192" lbl_NoError
        StrCmp "$DOTNET_RETURN_CODE" "error" lbl_Error
        StrCmp "$DOTNET_RETURN_CODE" "timeout" lbl_TimeOut
        ; It's a .Net Error
        StrCmp "$DOTNET_RETURN_CODE" "4101" lbl_Error_DuplicateInstance
        StrCmp "$DOTNET_RETURN_CODE" "4097" lbl_Error_NotAdministrator
        StrCmp "$DOTNET_RETURN_CODE" "1633" lbl_Error_InvalidPlatform lbl_FatalError
        ; all others are fatal

      lbl_Error_DuplicateInstance:
      DetailPrint "$(ERROR_DOTNET_DUPLICATE_INSTANCE)"
      GoTo lbl_Done

      lbl_Error_NotAdministrator:
      DetailPrint "$(ERROR_NOT_ADMINISTRATOR)"
      GoTo lbl_Done

      lbl_Error_InvalidPlatform:
      DetailPrint "$(ERROR_INVALID_PLATFORM)"
      GoTo lbl_Done

      lbl_TimeOut:
      DetailPrint "$(DESC_DOTNET_TIMEOUT)"
      GoTo lbl_Done

      lbl_Error:
      DetailPrint "$(ERROR_DOTNET_INVALID_PATH)"
      GoTo lbl_Done

      lbl_FatalError:
      DetailPrint "$(ERROR_DOTNET_FATAL)[$DOTNET_RETURN_CODE]"
      GoTo lbl_Done

      lbl_Done:
      DetailPrint "$(FAILED_DOTNET_INSTALL)"
      lbl_NoError:
      lbl_IsSilent:

  SectionEnd