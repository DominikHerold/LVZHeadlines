# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app

COPY . .

FROM build as publish
WORKDIR /build
RUN dotnet restore .
RUN dotnet publish -c Release --no-restore -o ./out

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS runtime
WORKDIR /app
COPY --from=publish /app/out ./

ENTRYPOINT ["/usr/bin/dotnet", "/app/LVZHeadlines.dll"]