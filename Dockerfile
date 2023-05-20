# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /source

# Copy everything and publish the release (publish implicitly restores and builds)
COPY *.csproj .
RUN dotnet restore --use-current-runtime

COPY . .
RUN dotnet publish --use-current-runtime --self-contained false --no-restore -o /app

# Label the container
LABEL maintainer="Dominik Herold"

# Label as GitHub action
LABEL com.github.actions.name="LVZ Headline scraper"
LABEL com.github.actions.description="Scrapes the Headlines of LVZ"
LABEL com.github.actions.icon="sliders"
LABEL com.github.actions.color="purple"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "LVZHeadlines.dll"]
