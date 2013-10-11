  Section "TeleInfo" DRIVER_TELEINFO
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_teleinfo.dll"
   ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\Ftdi_cpt.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\Ftdi_cpt.dll"
    ${EndIf}
  SectionEnd
