FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["game-presence/game-presence.csproj", "game-presence/"]
RUN dotnet restore "game-presence/game-presence.csproj"
COPY . .
WORKDIR "/src/game-presence"
RUN dotnet build "game-presence.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "game-presence.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "game-presence.dll"]
