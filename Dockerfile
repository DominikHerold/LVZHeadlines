FROM mcr.microsoft.com/dotnet/sdk:7.0.101 as build
WORKDIR /build

COPY . .

FROM build as publish
WORKDIR /build
RUN dotnet restore .
RUN dotnet publish -c Release --no-restore -o ./out

FROM mcr.microsoft.com/dotnet/sdk:7.0.101 AS runtime
WORKDIR /app
COPY --from=publish /build/out ./

ENTRYPOINT ["dotnet", "LVZHeadlines.dll"]