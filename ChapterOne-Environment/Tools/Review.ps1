param([switch]$CaptureOnly)
$ErrorActionPreference = 'Stop'
$projectFolder = Split-Path -Parent $PSScriptRoot
$exe = Join-Path $projectFolder 'Builds\ChapterOne\THAT NIGHT.exe'
$log = Join-Path $projectFolder 'Logs/review-player.log'
$arguments = '-environmentReview -screen-width 1280 -screen-height 720 -screen-fullscreen 0 -logFile "{0}"' -f $log
if ($CaptureOnly) { $arguments = '-batchmode ' + $arguments }
$windowStyle = if ($CaptureOnly) { 'Hidden' } else { 'Normal' }
$startedAt = [DateTime]::UtcNow
$process = Start-Process -FilePath $exe -ArgumentList $arguments -WindowStyle $windowStyle -PassThru -Wait
if ($process.ExitCode -ne 0) { throw "Review failed. Read $log" }
$resultPath = Join-Path $projectFolder 'Review\result.json'
if (-not (Test-Path -LiteralPath $resultPath) -or (Get-Item -LiteralPath $resultPath).LastWriteTimeUtc -lt $startedAt) { throw 'Review ended before producing a new result. Existing captures may be from an earlier build.' }
Get-Content -LiteralPath $resultPath -Raw
