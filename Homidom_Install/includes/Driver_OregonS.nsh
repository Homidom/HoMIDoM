  Section "OregonS" DRIVER_OregonS
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_OregonS.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\Oregon.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\Oregon.dll"
    ${EndIf}
  SectionEnd