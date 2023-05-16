FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY src/SolarEdge.Monitor/SolarEdge.Monitor.csproj SolarEdge.Monitor/
COPY nuget.config .
COPY lib/*.nupkg lib/
RUN dotnet restore SolarEdge.Monitor/SolarEdge.Monitor.csproj
COPY src/SolarEdge.Monitor/. SolarEdge.Monitor/
WORKDIR /src/SolarEdge.Monitor
RUN dotnet build SolarEdge.Monitor.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SolarEdge.Monitor.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SolarEdge.Monitor.dll"]
