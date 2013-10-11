  Section "USBuirt" DRIVER_USBUIRT
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_USBuirt.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-32bits\Drivers\UsbLibrary.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\UsbUirtManagedWrapper.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\uuirtdrv.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-64bits\Drivers\UsbLibrary.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\UsbUirtManagedWrapper.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\uuirtdrv.dll"
    ${EndIf}
  SectionEnd