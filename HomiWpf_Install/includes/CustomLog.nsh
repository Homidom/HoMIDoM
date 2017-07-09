var hFileLog
!macro Log_Init logfile
  FileOpen $hFileLog "${logfile}" w ;Or "a" for append
!macroend
!macro Log_String msg
  DetailPrint "${msg}"
  FileWrite $hFileLog "${msg}$\r$\n"
!macroend
!macro Log_Close
  FileWrite $hFileLog 'Done.'
  FileClose $hFileLog
!macroend
!macro File src
  File "${src}"
  FileWrite $hFileLog 'Extracting "${src}" to $outdir$\r$\n'
!macroend