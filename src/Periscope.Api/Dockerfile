#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Periscope.Api/Periscope.Api.csproj", "src/Periscope.Api/"]
COPY ["src/Periscope.Core/Periscope.Core.csproj", "src/Periscope.Core/"]
RUN dotnet restore "src/Periscope.Api/Periscope.Api.csproj"
COPY . .
WORKDIR "/src/src/Periscope.Api"
RUN dotnet build "Periscope.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Periscope.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Periscope.Api.dll"]