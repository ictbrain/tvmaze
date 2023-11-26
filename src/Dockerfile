FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY TvMaze.WebApi/*.csproj ./TvMaze.WebApi/
COPY TvMaze.Application/*.csproj ./TvMaze.Application/
COPY TvMaze.Database/*.csproj ./TvMaze.Database/
COPY TvMaze.Database.Migrations.Sqlite/*.csproj ./TvMaze.Database.Migrations.Sqlite/

RUN dotnet restore /app/TvMaze.WebApi/TvMaze.WebApi.csproj /p:Configuration=Release

# copy everything else and build app
COPY . .
WORKDIR /app/TvMaze.WebApi
RUN dotnet publish -r linux-musl-x64 --self-contained /p:PublishSingleFile=false -c Release -o /deploy

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.0-alpine3.18

RUN apk add --no-cache tzdata && rm -rf /var/cache/apk/* \
    && ln -sf /usr/share/zoneinfo/Europe/Amsterdam /etc/localtime \
    && echo "Europe/Amsterdam" > /etc/timezone

WORKDIR /app

COPY --from=build /deploy  .

# create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser:dotnetuser /app

# impersonate into the new user
USER dotnetuser

EXPOSE 8080
ENTRYPOINT ["./TvMaze.WebApi"]