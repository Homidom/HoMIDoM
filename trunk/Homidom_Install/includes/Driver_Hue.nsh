  Section "HUE" DRIVER_HUE
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_hue.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\AsyncCtpLibrary.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Q42.HueApi.dll"
      File  "..\Dll_externes\Homidom-64bits\Newtonsoft.Json.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\AsyncCtpLibrary.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Q42.HueApi.dll"
      File  "..\Dll_externes\Homidom-32bits\Newtonsoft.Json.dll"
    ${EndIf}
  SectionEnd