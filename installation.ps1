$destinationDir = Join-Path $env:USERPROFILE ".yail"

if (-not (Test-Path -Path $destinationDir)) {
    New-Item -ItemType Directory -Path $destinationDir | Out-Null
}

$currentDirectory = Get-Location
Write-Host "Copying files from $currentDirectory to $destinationDir"

Get-ChildItem -File | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $destinationDir -Force
}

Get-ChildItem -Recurse -Directory | ForEach-Object {
    $destinationPath = Join-Path -Path $destinationDir -ChildPath $_.FullName.Substring($PWD.Path.Length + 1)

    if (-not (Test-Path -Path $destinationPath)) {
        New-Item -Path $destinationPath -ItemType Directory
    }

    Get-ChildItem -Path $_.FullName | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $destinationPath -Force
    }
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