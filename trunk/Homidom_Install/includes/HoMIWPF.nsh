InstType "Client WPF uniquement"
Section "HoMIDoM Client WPF" HoMIDoM_WPFCLIENT

  SectionIn 1 4

  SetOverwrite on
  SetOutPath "$INSTDIR\HoMIWpf"
  CreateDirectory "$INSTDIR\HoMIWpf\Logs"
  CreateDirectory "$INSTDIR\HoMIWpf\Images"
  CreateDirectory "$INSTDIR\HoMIWpf\data"
  
  File "..\DEBUG_HoMIWpF\*"
  SetOutPath "$INSTDIR\HoMIWpf\Images"
  File /r "..\DEBUG_HoMIWpF\Images\*"
  
  SetOutPath "$INSTDIR\HoMIWpf\data"
  File /r "..\DEBUG_HoMIWpF\data\*"

  SetOverwrite off
  CreateDirectory "$INSTDIR\HoMIWpf\Config"
  ;SetOutPath "$INSTDIR\HoMIWpf\Config"
  ;File "..\DEBUG_HoMIWpF\Config\*"

  CreateShortCut $SMPROGRAMS\${PRODUCT_NAME}\HoMIDoM WPF Client.lnk $INSTDIR\HoMIWpf\HoMIWpF.exe

SectionEnd