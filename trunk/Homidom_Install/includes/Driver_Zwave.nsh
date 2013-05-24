  Section "Zwave" DRIVER_ZWAVE
    SectionIn 1 2
    SetOutPath "$INSTDIR\Drivers"
    File "..\RELEASE\Drivers\Driver_Zwave.dll"

    ${If} ${RunningX64}
      File  "..\Dll_externes\Homidom-64bits\Drivers\OpenZWaveDotNet.dll"
    ${Else}
      File  "..\Dll_externes\Homidom-32bits\Drivers\OpenZWaveDotNet.dll"
    ${EndIf}
    
    CreateDirectory "$INSTDIR\Drivers\ZWave"
    SetOutPath "$INSTDIR\Drivers\ZWave"
    File /r "..\RELEASE\Drivers\Zwave\*"
    


  SectionEnd