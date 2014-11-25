  Section "Google Calendar" DRIVER_GoogleCalendar
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_GoogleCalendar.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\Google.GData*.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\Google.GData*.dll"
    ${EndIf}
  SectionEnd