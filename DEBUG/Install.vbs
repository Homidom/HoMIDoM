Set objFSO = CreateObject("Scripting.FileSystemObject")

If IsObject(Session) then
	targetDir = Session.Property("CustomActionData")
Else
	targetDir = objFSO.GetAbsolutePathName(".")&"\"
End If

strComputer = "."
Set objWMIService = GetObject("winmgmts:" & "{impersonationLevel=impersonate}!\\" & strComputer & "\root\cimv2")
Set colProcessor = objWMIService.ExecQuery("Select * from Win32_Processor")
For Each objProcessor in colProcessor
	iAddressWidth = objProcessor.AddressWidth
Next
If iAddressWidth = 32 Then
	GetArchitecture = "x86"
ElseIf iAddressWidth = 64 Then
	GetArchitecture = "x64"
End If

If GetArchitecture = "x64" Then
	If objFSO.FolderExists(targetDir) Then
		For each oFl in  objFSO.GetFolder(targetDir).Files
			If Instr(oFl.Name,".x64") <> 0 Then
				x64file = targetDir&oFl.Name
				dllfile = targetDir&Left(oFl.Name,Len(oFl.Name)-4)
				If objFSO.FileExists(dllfile) Then objFSO.DeleteFile dllfile
				If objFSO.FileExists(x64file) Then objFSO.MoveFile x64file, dllfile
			End If
		Next
	End If
	If objFSO.FolderExists(targetDir&"Drivers") Then
		For each oFl in  objFSO.GetFolder(targetDir&"Drivers").Files
			If Instr(oFl.Name,".x64") <> 0 Then
				x64file = targetDir&"Drivers\"&oFl.Name
				dllfile = targetDir&"Drivers\"&Left(oFl.Name,Len(oFl.Name)-4)
				If objFSO.FileExists(dllfile) Then objFSO.DeleteFile dllfile
				objFSO.MoveFile x64file, dllfile
			End If
		Next
	End If
End If


