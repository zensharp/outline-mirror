FROM mcr.microsoft.com/dotnet/sdk

WORKDIR /source
ENV PATH="$PATH:/source/publish"

RUN dotnet tool install --global dotnet-script
ENV PATH="$PATH:/root/.dotnet/tools"

COPY scripts/ ./scripts
RUN dotnet script publish "scripts/main.csx" --output "publish" --name "outline-mirror"
