  Section "Google Calendar" DRIVER_GoogleCalendar
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_GoogleCalendar.dll"
    File "..\RELEASE\Drivers\Google.GData.AccessControl.dll"
    File "..\RELEASE\Drivers\Google.GData.Calendar.dll"
    File "..\RELEASE\Drivers\Google.GData.Client.dll"
    File "..\RELEASE\Drivers\Google.GData.Extensions.dll"
  SectionEnd