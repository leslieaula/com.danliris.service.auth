nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
nuget install -Verbosity quiet -OutputDirectory packages -Version 2.4.5.0 ReportGenerator

OPENCOVER=$PWD/packages/OpenCover.4.6.519/tools/OpenCover.Console.exe
REPORTGENERATOR=$PWD/packages/ReportGenerator.2.4.5.0/tools/ReportGenerator.exe

coverage=./coverage
rm -rf $coverage
mkdir $coverage

$OPENCOVER \
  -target:"C:\Program Files\dotnet\dotnet.exe" \
  -targetargs:"test -f Com.DanLiris.Service.Auth.Test/Com.DanLiris.Service.Auth.Test.csproj" \
  -mergeoutput \
  -hideskipped:File \
  -output:$coverage/coverage.xml \
  -oldStyle \
  -filter:"+[*]*" \
  -register:user

$REPORTGENERATOR \
  -reports:$coverage/coverage.xml \
  -targetdir:$coverage \
  -verbosity:Error