$ErrorActionPreference = 'Stop'
$projectFolder = Split-Path -Parent $PSScriptRoot
$editor = 'C:\Users\USER\AppData\Local\Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe'
$log = Join-Path $projectFolder 'Logs/build.log'
if (-not (Test-Path -LiteralPath $editor)) { throw 'Install Unity 6000.3.28f1 or update the editor path in Tools/Build.ps1.' }
$openEditor = Get-CimInstance Win32_Process -Filter "Name = 'Unity.exe'" | Where-Object { $_.CommandLine -like '*-projectPath*' -and $_.CommandLine -like "*$projectFolder*" -and $_.CommandLine -notlike '*-adb2*' }
if ($openEditor) {
    $result = Join-Path $projectFolder 'Logs/editor-result.txt'
    Set-Content -LiteralPath $result -Value 'PENDING build'
    Set-Content -LiteralPath (Join-Path $projectFolder 'Tools/editor-request.txt') -Value 'build'
    $deadline = (Get-Date).AddMinutes(5)
    while ((Get-Date) -lt $deadline) {
        Start-Sleep -Seconds 1
        $status = Get-Content -LiteralPath $result -Raw
        if ($status.StartsWith('ERROR')) { throw $status }
        if ($status.StartsWith('OK build')) { Write-Output (Join-Path $projectFolder 'Builds/ChapterOneEnvironment.exe'); exit 0 }
    }
    throw 'Unity has not processed the build request. Bring its window forward and check the Console.'
}
$arguments = '-batchmode -nographics -quit -projectPath "{0}" -buildTarget StandaloneWindows64 -executeMethod ThatNight.ChapterOne.Editor.EnvironmentBuild.Build -logFile "{1}"' -f $projectFolder,$log
$process = Start-Process -FilePath $editor -ArgumentList $arguments -WindowStyle Hidden -PassThru -Wait
if ($process.ExitCode -ne 0) { throw "Unity build failed. Read $log" }
Write-Output (Join-Path $projectFolder 'Builds\ChapterOneEnvironment.exe')
