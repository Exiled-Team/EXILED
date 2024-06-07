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
 
Move-File -sourcePath ".\EXILED-DLL-Archiver.exe" -destinationPath ".\bin\Release\EXILED-DLL-Archiver.exe"
Move-File -sourcePath ".\References\Mono.Posix.dll" -destinationPath ".\bin\Release\Mono.Posix.dll"
Move-File -sourcePath ".\References\System.ComponentModel.DataAnnotations.dll" -destinationPath ".\bin\Release\System.ComponentModel.DataAnnotations.dll"
CD .\bin\Release
.\EXILED-DLL-Archiver.exe
