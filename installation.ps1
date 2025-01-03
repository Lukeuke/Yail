$destinationDir = Join-Path $env:USERPROFILE ".yail"

if (-not (Test-Path -Path $destinationDir)) {
    New-Item -ItemType Directory -Path $destinationDir | Out-Null
}

$currentDirectory = Get-Location
Write-Host "Copying files from $currentDirectory to $destinationDir"

Get-ChildItem -File | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $destinationDir -Force
}

Write-Host "Yail installed."

$currentPath = [Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::User)
if (-not ($currentPath -like "*$destinationDir*")) {
    [Environment]::SetEnvironmentVariable(
        "Path",
        $currentPath + ";$destinationDir",
        [EnvironmentVariableTarget]::User
    )
    Write-Host "The .yail directory has been added to your PATH. Restart your terminal to use it."
} else {
    Write-Host "The .yail directory is already in the PATH."
}

Write-Host "Press any key to exit..."
[System.Console]::ReadKey() | Out-Null