Option Explicit

Dim Fso, testPath

function CleanUp
    testPath= Session.Property("INSTALLDIR") + "UAB"

    Set Fso = CreateObject("Scripting.FileSystemObject")
     
    if Fso.FolderExists(testPath) Then
        Fso.DeleteFolder( testPath )
    End If
end function

