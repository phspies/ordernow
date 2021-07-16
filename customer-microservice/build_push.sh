version=v1.4
dotnet build --configuration Release
docker build -f ./Dockerfile.Release -t silmaril212/customermicroservice:$version . 
docker push silmaril212/customermicroservice:$version

