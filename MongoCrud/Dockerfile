#use docker compose

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


#TempConnectionstring
ENV INIT_VAR="mongodb://root:example@mongodb:27017"
#Tempkey
ENV API_KEY="32B66F391C7142F994974A99C509817B"

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN dotnet restore "./MongoCrud/MongoCrudPeopleApi.csproj"
COPY . .
WORKDIR "/src/MongoCrud"
RUN dotnet build "./MongoCrudPeopleApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MongoCrudPeopleApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MongoCrudPeopleApi.dll"]