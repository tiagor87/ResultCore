#nuget install xunit.runner.console -OutputDirectory packages -Version 2.4.1

nuget install OpenCover -OutputDirectory packages -Version 4.7.922

dotnet tool install coveralls.net --tool-path packages

dotnet build

.\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe -target:"dotnet.exe" -targetargs:"test "".\tests\UnitTests\ResultCore.UnitTests.csproj\"" --configuration Debug --no-build" -filter:"+[ResultCore]* -[ResultCore*Tests]*" -oldStyle -output:"coverage.xml"

.\packages\csmacnz.coveralls.exe --opencover -i coverage.xml --repoToken $env:COVERALLS_REPO_TOKEN