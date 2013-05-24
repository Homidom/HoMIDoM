  Section "Phidget" DRIVER_PHIDGET
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Phidget.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\Phidget21.NET.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\Phidget21.NET.dll"
    ${EndIf}
  SectionEnd