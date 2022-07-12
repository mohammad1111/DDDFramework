$nuget_archive = "nuget-archive"
$source = "http://172.31.0.156:1500/v3/index.json"
$key = "X2eSDCBwnVWumXx3"

Get-ChildItem -Path $nuget_archive -Recurse -Filter *.nupkg | ForEach-Object {
    Write-Host "About to push a package named '$( $_.Name )'..."
    dotnet nuget push $_.FullName --source $source --api-key $key --skip-duplicate
}

Write-Host "Finished!"