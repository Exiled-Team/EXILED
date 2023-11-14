<#
.SYNOPSIS
Moves a file from one location to another.
 
.DESCRIPTION
This function moves a file from the source location to the destination location.
 
.PARAMETER sourcePath
The path of the file to be moved.
 
.PARAMETER destinationPath
The path where the file should be moved to.
 
.EXAMPLE
Move-File -sourcePath "C:\Temp\file.txt" -destinationPath "D:\Backup\file.txt"
Moves the file "C:\Temp\file.txt" to "D:\Backup\file.txt".
#>
function Move-File {
    param (
        [Parameter(Mandatory=$true)]
        [string]$sourcePath,
 
        [Parameter(Mandatory=$true)]
        [string]$destinationPath
    )
 
    # Check if the source file exists
    if (-not (Test-Path -Path $sourcePath -PathType Leaf)) {
        Write-Output "Source file does not exist."
        return
    }
 
    # Check if the destination directory exists
    $destinationDirectory = Split-Path -Path $destinationPath
    if (-not (Test-Path -Path $destinationDirectory -PathType Container)) {
        Write-Output "Destination directory does not exist."
        return
    }
 
    # Move the file to the destination
    try {
        Move-Item -Path $sourcePath -Destination $destinationPath -Force
        Write-Output "File moved successfully."
    }
    catch {
        Write-Output "Failed to move the file: $_"
    }
}
 
# Usage example for the Move-File function
 
# Move a file from one location to another
Move-File -sourcePath ".\EXILED-DLL-Archiver.exe" -destinationPath ".\bin\Release\EXILED-DLL-Archiver.exe"
Move-File -sourcePath ".\References\0Harmony.dll" -destinationPath ".\bin\Release\0Harmony.dll"
Move-File -sourcePath ".\References\SemanticVersioning.dll" -destinationPath ".\bin\Release\SemanticVersioning.dll"
Move-File -sourcePath ".\References\Mono.Posix.dll" -destinationPath ".\bin\Release\Mono.Posix.dll"
Move-File -sourcePath ".\References\System.ComponentModel.DataAnnotations.dll" -destinationPath ".\bin\Release\System.ComponentModel.DataAnnotations.dll"
