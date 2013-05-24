  Section "X10" DRIVERS_X10
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_X10_CM11.dll"
    File "..\RELEASE\Drivers\Driver_X10_CM15.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\Cm11.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\Cm11.dll"
    ${EndIf}
  SectionEnd