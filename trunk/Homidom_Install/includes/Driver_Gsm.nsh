  Section "GSM" DRIVER_GSM
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Gsm.dll"
    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\GSMComm*.dll"
      File  "..\Dll_externes\Homidom-64bits\Drivers\PDUConverter.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\GSMComm*.dll"
      File  "..\Dll_externes\Homidom-32bits\Drivers\PDUConverter.dll"
    ${EndIf}
  SectionEnd