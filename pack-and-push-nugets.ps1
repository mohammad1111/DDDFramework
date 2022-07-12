$nuget_output = "nuget-output"
$source = "http://172.31.0.156:1500/v3/index.json"
$key = "X2eSDCBwnVWumXx3"

Remove-Item $nuget_output -Recurse -Force

dotnet pack Gig.Framework.Api
dotnet pack Gig.Framework.Application
dotnet pack Gig.Framework.Apllication.ReadModel
dotnet pack Gig.Framework.Bus
dotnet pack Gig.Framework.Caching
dotnet pack Gig.Framework.Config
dotnet pack Gig.Framework.Core
dotnet pack Gig.Framework.Data.Elastic
dotnet pack Gig.Framework.DependencyInjection.Autofac
#dotnet pack ..\Gig.Framework.DependencyInjection.Windsor
dotnet pack Gig.Framework.Domain
dotnet pack Gig.Framework.Facade
dotnet pack Gig.Framework.Persistence.Ef
dotnet pack Gig.Framework.ReadModel
dotnet pack Gig.Framework.RuleEngin
dotnet pack Gig.Framework.RuleEngine.Contract
dotnet pack Gig.Framework.Scheduling
dotnet pack Gig.Framework.Security
dotnet pack Gig.Framework.Workflow
dotnet pack Gig.Framework.Workflow.Contract
dotnet pack Gig.Framework.TestUtilities

Get-ChildItem -Path $nuget_output -Filter *.nupkg | ForEach-Object {
    Write-Host "About to push a package named '$( $_.Name )'..."
    dotnet nuget push $_.FullName --source $source --api-key $key --skip-duplicate
}

Write-Host "Finished!"
