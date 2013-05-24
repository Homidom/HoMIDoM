  Section "ModbusTCP" DRIVER_ModbusTCP
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_ModbusTCP.dll"
  SectionEnd