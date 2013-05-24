  Section "Arduino" DRIVER_ARDUINO
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Arduino.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\FirmataVB.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\FirmataVB.dll"
    ${EndIf}
  SectionEnd