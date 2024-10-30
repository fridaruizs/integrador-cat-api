# stage 1
FROM mcr.microsoft.com/dotnet/sdk:8.0.100 AS build
WORKDIR /src

# project files
COPY ["integrador-cat-api.sln", "./"]
COPY ["integrador-cat-api/integrador-cat-api.csproj", "integrador-cat-api/"]
COPY ["cat-api.Tests/cat-api.Tests.csproj", "cat-api.Tests/"]

# dependencies
RUN dotnet restore

COPY . .

# build and test
RUN dotnet build -c Release --no-restore
RUN dotnet test -c Release --no-build --verbosity normal

# publish
RUN dotnet publish "integrador-cat-api/integrador-cat-api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore


# stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0.0 AS final
WORKDIR /app
EXPOSE 80
# EXPOSE 443

# non-root user
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

# curl for healthcheck
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER appuser

# env variables
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true

HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/healthcheck || exit 1

CMD ["dotnet", "integrador-cat-api.dll"]