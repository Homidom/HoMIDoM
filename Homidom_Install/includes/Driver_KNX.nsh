  Section "KNX/EIBD" DRIVER_KNX
    SectionIn 1 2
    CreateDirectory "$INSTDIR\Drivers\KNX"
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_KNX.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\KNX*.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\KNX*.dll"
    ${EndIf}
  SectionEnd