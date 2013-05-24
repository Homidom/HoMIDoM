  Section "OpenWebNet" DRIVERS_OpenWebNet
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_OpenWebNet_IP.dll"
    File "..\RELEASE\Drivers\Driver_OpenWebNet_USB.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\OpenWebNet.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\OpenWebNet.dll"
    ${EndIf}
  SectionEnd