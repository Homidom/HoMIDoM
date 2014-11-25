  Section "Mirror" DRIVER_MIRROR
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Mirror.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\UsbLibrary.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\UsbLibrary.dll"
    ${EndIf}
  SectionEnd