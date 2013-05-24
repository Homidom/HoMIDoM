  Section "HomeSeer" DRIVER_HS
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_HomeSeer.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\HomeSeer2.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\HS2Util.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\Scheduler.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\HomeSeer2.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\HS2Util.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\Scheduler.dll"
    ${EndIf}
  SectionEnd