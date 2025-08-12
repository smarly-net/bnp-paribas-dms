# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["./dms.sln", "./"] 
COPY ["./src/dms.domain/dms.domain.csproj", "./src/dms.domain/"]
COPY ["./src/dms.contracts/dms.contracts.csproj", "./src/dms.contracts/"]
COPY ["./src/dms.application.abstractions/dms.application.abstractions.csproj", "./src/dms.application.abstractions/"]
COPY ["./src/dms.application/dms.application.csproj", "./src/dms.application/"]
COPY ["./src/dms.infrastructure.read/dms.infrastructure.read.csproj", "./src/dms.infrastructure.read/"]
COPY ["./src/dms.infrastructure.write/dms.infrastructure.write.csproj", "./src/dms.infrastructure.write/"]
COPY ["./src/dms.web/dms.web.csproj", "./src/dms.web/"]
COPY ["./test/service/dms.web.test.service/dms.web.test.service.csproj", "./test/service/dms.web.test.service/"]
COPY ["./test/unit/dms.web.test.unit/dms.web.test.unit.csproj", "./test/unit/dms.web.test.unit/"]
RUN dotnet restore ./dms.sln

COPY . .

RUN dotnet build ./dms.sln -c $BUILD_CONFIGURATION --no-restore
RUN dotnet test  ./dms.sln -c $BUILD_CONFIGURATION --no-build --no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./src/dms.web/dms.web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

ENV ReadDatabase__ConnectionString="Data Source=/app/db/readDb.sqlite"
RUN mkdir -p /app/db
COPY ["./db/readDb.sqlite", "/app/db/readDb.sqlite"]

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dms.web.dll"]