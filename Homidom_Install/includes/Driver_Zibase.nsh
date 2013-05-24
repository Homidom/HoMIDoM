  Section "Zibase" DRIVER_ZIBASE
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Zibase.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\ZibaseDll.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\ZibaseDll.dll"
    ${EndIf}
  SectionEnd