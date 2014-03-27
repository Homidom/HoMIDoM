  Section "Mail" DRIVER_MAIL
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_mail.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\NLog.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\S22.Imap.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\NLog.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\S22.Imap.dll"
    ${EndIf}
  SectionEnd
