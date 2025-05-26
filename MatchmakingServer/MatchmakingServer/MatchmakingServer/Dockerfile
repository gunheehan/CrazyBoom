# 1. 기본 런타임 (앱 실행 단계)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7800

# 2. SDK (빌드 단계)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

#필수 패키지 설치
RUN apt-get update && apt-get install -y \
    clang \
    gcc \
    zlib1g-dev

COPY ["MatchmakingServer.csproj", "."]
RUN dotnet restore "MatchmakingServer.csproj"
COPY . .
#WORKDIR "/src/MatchmakingServer"
RUN dotnet build "MatchmakingServer.csproj" -c $BUILD_CONFIGURATION -o /app/buiild

# 3. Publish 단계
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MatchmakingServer.csproj" -c $BUILD_CONFIGURATION -o /app/pub>

# 4. 최종 실행 이미지
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MatchmakingServer.dll"]