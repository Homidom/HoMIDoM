  Section "RFXCOM" DRIVER_RFXCOM
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_RFXMitter.dll"
    File "..\RELEASE\Drivers\Driver_RFXReceiver.dll"
    File "..\RELEASE\Drivers\Driver_RFXtrx.dll"
  SectionEnd