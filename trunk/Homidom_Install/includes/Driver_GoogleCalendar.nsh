  Section "Google Calendar" DRIVER_GoogleCalendar
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_GoogleCalendar.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\Google.Apis.Auth.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Google.Apis.Auth.PlatformServices.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Google.Apis.Calendar.v3.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Google.Apis.Core.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Google.Apis.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Google.Apis.PlatformServices.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Microsoft.Threading.Tasks.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Microsoft.Threading.Tasks.Extensions.Desktop"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Newtonsoft.Json.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\System.Threading.Tasks"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\Google.Apis.Auth.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Google.Apis.Auth.PlatformServices.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Google.Apis.Calendar.v3.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Google.Apis.Core.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Google.Apis.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Google.Apis.PlatformServices.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Microsoft.Threading.Tasks.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Microsoft.Threading.Tasks.Extensions.Desktop"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Newtonsoft.Json.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\System.Threading.Tasks"
    ${EndIf}
  SectionEnd