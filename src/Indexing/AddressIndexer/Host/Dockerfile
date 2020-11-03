#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Indexing/AddressIndexer/Host/Host.csproj", "src/Indexing/AddressIndexer/Host/"]
COPY ["src/Shared/Data/Data.csproj", "src/Shared/Data/"]
COPY ["src/Shared/Documents/Documents.csproj", "src/Shared/Documents/"]
COPY ["src/Shared/Core/Core.csproj", "src/Shared/Core/"]
COPY ["src/Indexing/HostBase/HostBase.csproj", "src/Indexing/HostBase/"]
COPY ["src/Shared/Stats/Stats.csproj", "src/Shared/Stats/"]
COPY ["src/Shared/Events/Events.csproj", "src/Shared/Events/"]
RUN dotnet restore "src/Indexing/AddressIndexer/Host/Host.csproj"
COPY . .
WORKDIR "/src/src/Indexing/AddressIndexer/Host"
RUN dotnet build "Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Periscope.Indexing.AddressIndexer.Host.dll"]