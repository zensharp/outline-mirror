FROM mcr.microsoft.com/dotnet/sdk

WORKDIR /source

RUN apt-get update
RUN apt-get install openssh-client -y

ENV PATH="$PATH:/source/publish"
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-script

COPY scripts/ ./scripts
RUN dotnet script publish "scripts/main.csx" --output "publish" --name "outline-mirror"
