$source = "http://172.31.0.156:1500/v3/index.json"
$key = "X2eSDCBwnVWumXx3"
$packages = @(
"Gig.Framework.Api",
"Gig.Framework.Application",
"Gig.Framework.Application.ReadModel",
"Gig.Framework.Bus",
"Gig.Framework.Caching",
"Gig.Framework.Config",
"Gig.Framework.Core",
"Gig.Framework.Data.Elastic",
"Gig.Framework.DependencyInjection.Autofac",
"Gig.Framework.DependencyInjection.Windsor",
"Gig.Framework.Domain",
"Gig.Framework.Facade",
"Gig.Framework.Persistence.Ef",
"Gig.Framework.ReadModel",
"Gig.Framework.RuleEngine",
"Gig.Framework.RuleEngine.Contract",
"Gig.Framework.Scheduling",
"Gig.Framework.Security",
"Gig.Framework.Workflow",
"Gig.Framework.Workflow.Contract",
"Gig.Framework.TestUtilities"
)
$versions = @("2.7.0.37" )

foreach ($version in $versions)
{
    foreach ($package in $packages)
    {
        dotnet nuget delete $package $version --source $source --api-key $key --non-interactive
    }
}

Write-Host "Finished!"