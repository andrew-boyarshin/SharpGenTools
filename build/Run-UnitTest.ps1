Param(
    [string] $Project,
    [string] $Configuration,
    [bool] $CollectCoverage,
    [string] $RepoRoot,
    [string] $TestSubdirectory = ""
)

dotnet test "$RepoRoot/$TestSubdirectory/$Project/$Project.csproj" --no-build --no-restore -c $Configuration `
    /p:Include='[SharpGen]*%2c[SharpGen.Runtime]*%2c[SharpGen.Platform]*%2c[SharpGen.Extension.MicrosoftDocs]*%2c[SharpGenTools.Sdk]*' `
    /p:CollectCoverage=$CollectCoverage /p:CoverletOutputFormat=opencover `
    /p:CoverletOutput="$RepoRoot/artifacts/coverage/$Project.xml" --logger "trx;LogFileName=$RepoRoot/artifacts/test-results/$Project.trx" `
    | Write-Host

return $LastExitCode -eq 0
