  Section "Teracom" DRIVER_Teracom
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Teracom.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\SharpSnmpLib.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\SharpSnmpLib.dll"
    ${EndIf}
  SectionEnd