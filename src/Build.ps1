cd src
$build = $Env:APPVEYOR_BUILD_NUMBER
dotnet build
dotnet test
dotnet pack -p:PackageVersion=0.0.$build
dotnet nuget push **/Booster.0.0.$build.nupkg -k oy2k76abznezhudctcvqmiegfvhfdcau5a5l6l36yflkbq -s "https://nuget.org"
