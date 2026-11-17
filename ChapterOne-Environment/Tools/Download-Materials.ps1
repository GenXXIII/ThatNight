param([string[]]$AssetIds=@('Ground054','Ground084','Ground037','Planks037A','RoofingTiles014B','WoodFloor051','Wood066','Fabric035','Metal033'))
$ErrorActionPreference = 'Stop'
$projectFolder = Split-Path -Parent $PSScriptRoot
$cache = Join-Path $PSScriptRoot 'MaterialCache'
New-Item -ItemType Directory -Path $cache -Force | Out-Null
Add-Type -AssemblyName System.IO.Compression.FileSystem
$ids = $AssetIds
$catalog = Invoke-RestMethod -Uri ('https://ambientcg.com/api/v3/assets?id=' + ($ids -join ',') + '&include=downloads')
foreach ($asset in $catalog.assets) {
    $download = $asset.downloads | Where-Object attributes -eq '1K-JPG' | Select-Object -First 1
    if (-not $download) { throw ('Missing 1K material: ' + $asset.id) }
    $archive = Join-Path $cache ($asset.id + '.zip')
    if (-not (Test-Path -LiteralPath $archive)) { Invoke-WebRequest -Uri $download.url -OutFile $archive -UseBasicParsing }
    $target = Join-Path $projectFolder ('Assets/ChapterOne/Textures/' + $asset.id)
    New-Item -ItemType Directory -Path $target -Force | Out-Null
    $zip = [IO.Compression.ZipFile]::OpenRead($archive)
    try {
        foreach ($entry in $zip.Entries) {
            if ($entry.Name -match '\.(jpg|png|txt)$') {
                [IO.Compression.ZipFileExtensions]::ExtractToFile($entry,(Join-Path $target $entry.Name),$true)
            }
        }
    } finally { $zip.Dispose() }
    Write-Output ('Prepared ' + $asset.id)
}
