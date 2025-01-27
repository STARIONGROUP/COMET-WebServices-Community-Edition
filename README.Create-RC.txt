docker pull mcr.microsoft.com/dotnet/sdk:9.0
DOCKER_BUILDKIT=1 docker build -f Dockerfile -t stariongroup/comet-webservices-community-edition:10.0.0-rc1 .
docker push stariongroup/comet-webservices-community-edition:10.0.0-rc1


Pre-Release having a SDK pre release on GitHub:

export SDK_PRE_RELEASE_NUGET_USERNAME=<GitHub username>
export SDK_PRE_RELEASE_NUGET_PASSWORD=<GitHub token>
DOCKER_BUILDKIT=1 docker build --secret id=SDK_PRE_RELEASE_NUGET_USERNAME,env=SDK_PRE_RELEASE_NUGET_USERNAME --secret id=SDK_PRE_RELEASE_NUGET_PASSWORD,env=SDK_PRE_RELEASE_NUGET_PASSWORD  -f DockerfilePreRelease -t stariongroup/comet-webservices-community-edition:10.0.0-rc1 .