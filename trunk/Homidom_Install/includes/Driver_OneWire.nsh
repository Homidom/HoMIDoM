  Section "OneWire" DRIVER_1WIRE
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_onewire.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\OneWireAPI.NET.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\OneWireAPI.NET.dll"
    ${EndIf}
  SectionEnd
