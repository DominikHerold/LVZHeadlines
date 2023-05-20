# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /app

COPY . .

RUN dotnet restore .
RUN dotnet publish -c Release --no-restore -o ./out

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS runtime
WORKDIR /app
COPY --from=build-env /app/out ./

# to run locally change to:
# ENTRYPOINT ["dotnet", "/app/LVZHeadlines.dll"]
# see https://svrooij.io/2022/06/09/building-github-action-in-net/
ENTRYPOINT ["/usr/bin/dotnet", "/app/LVZHeadlines.dll"]