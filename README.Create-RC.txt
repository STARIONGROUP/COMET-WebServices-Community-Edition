docker pull mcr.microsoft.com/dotnet/sdk:8.0
DOCKER_BUILDKIT=1 docker build -f Dockerfile -t stariongroup/comet-webservices-community-edition:8.0.0-rc37 .
docker push stariongroup/comet-webservices-community-edition:8.0.0-rc37