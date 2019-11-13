cd src
docker ps
dotnet build
dotnet pack 
dotnet nuget push **/Booster.0.0.**.nupkg -k oy2k76abznezhudctcvqmiegfvhfdcau5a5l6l36yflkbq -s "https://nuget.org"