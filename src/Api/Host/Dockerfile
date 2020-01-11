#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Api/Host/Host.csproj", "src/Api/Host/"]
COPY ["src/Api/Application/Application.csproj", "src/Api/Application/"]
COPY ["src/Shared/Data/Data.csproj", "src/Shared/Data/"]
COPY ["src/Shared/Documents/Documents.csproj", "src/Shared/Documents/"]
COPY ["src/Shared/Core/Core.csproj", "src/Shared/Core/"]
COPY ["src/Api/Search/Search.csproj", "src/Api/Search/"]
COPY ["src/Shared/Stats/Stats.csproj", "src/Shared/Stats/"]
RUN dotnet restore "src/Api/Host/Host.csproj"
COPY . .
WORKDIR "/src/src/Api/Host"
RUN dotnet build "Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cinder.Api.Host.dll"]