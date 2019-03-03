nuget install OpenCover -Version 4.6.519 -OutputDirectory coverage
nuget install coveralls.net -Version 0.6.0 -OutputDirectory coverage
dotnet restore .\tests\UnitTests\
dotnet build .\tests\UnitTests\
.\coverage\OpenCover.4.6.519\tools\Opencover.console.exe -register:user -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test "".\tests\UnitTests"" -f netcoreapp2.0" -filter:"+[*]* -[*Tests]*" -returntargetcode -output:"coverage.xml" -oldStyle
.\coverage\coveralls.net.0.6.0\tools\csmacnz.Coveralls.exe --opencover -i .\coverage.xml
