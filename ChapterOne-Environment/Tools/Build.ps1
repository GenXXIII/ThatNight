$ErrorActionPreference = 'Stop'
$projectFolder = Split-Path -Parent $PSScriptRoot
$editor = @(
    'C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe',
    (Join-Path $env:LOCALAPPDATA 'Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe')
) | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
$log = Join-Path $projectFolder 'Logs/build.log'
New-Item -ItemType Directory -Path (Join-Path $projectFolder 'Logs') -Force | Out-Null
if (-not $editor) { throw 'Install Unity 6000.3.28f1 or update the editor path in Tools/Build.ps1.' }
$openEditor = Get-CimInstance Win32_Process -Filter "Name = 'Unity.exe'" | Where-Object { $_.CommandLine -like '*-projectPath*' -and $_.CommandLine -like "*$projectFolder*" -and $_.CommandLine -notlike '*-adb2*' }
if ($openEditor) {
    $result = Join-Path $projectFolder 'Logs/editor-result.txt'
    $request = Join-Path $projectFolder 'Tools/editor-request.txt'
    if (Test-Path -LiteralPath $request) { throw 'An editor request is already pending. Wait for it to finish.' }
    Set-Content -LiteralPath $result -Value 'PENDING build'
    Set-Content -LiteralPath ($request + '.tmp') -Value 'build'
    Move-Item -LiteralPath ($request + '.tmp') -Destination $request
    $deadline = (Get-Date).AddMinutes(20)
    while ((Get-Date) -lt $deadline) {
        Start-Sleep -Seconds 1
        $status = Get-Content -LiteralPath $result -Raw
        if ($status.StartsWith('ERROR')) { throw $status }
        if ($status.StartsWith('OK build')) { Write-Output (Join-Path $projectFolder 'Builds/ChapterOne/THAT NIGHT.exe'); exit 0 }
    }
    throw 'Unity has not processed the build request. Bring its window forward and check the Console.'
}
$arguments = '-batchmode -nographics -quit -projectPath "{0}" -buildTarget StandaloneWindows64 -executeMethod ThatNight.ChapterOne.Editor.EnvironmentBuild.Build -logFile "{1}"' -f $projectFolder,$log
$process = Start-Process -FilePath $editor -ArgumentList $arguments -WindowStyle Hidden -PassThru -Wait
if ($process.ExitCode -ne 0) { throw "Unity build failed. Read $log" }
Write-Output (Join-Path $projectFolder 'Builds/ChapterOne/THAT NIGHT.exe')
