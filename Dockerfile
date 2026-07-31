# Build stage: full SDK, compiles and publishes the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project files first so `dotnet restore` is cached
# independently of source-code changes.
COPY src/AdjacentPairCounter.Api/AdjacentPairCounter.Api.csproj src/AdjacentPairCounter.Api/
COPY src/AdjacentPairCounter.Application/AdjacentPairCounter.Application.csproj src/AdjacentPairCounter.Application/
COPY src/AdjacentPairCounter.Domain/AdjacentPairCounter.Domain.csproj src/AdjacentPairCounter.Domain/

RUN dotnet restore src/AdjacentPairCounter.Api/AdjacentPairCounter.Api.csproj

# Now bring in the actual source and publish. The Tests project is
# deliberately never copied into the image — it isn't part of what ships.
COPY src/AdjacentPairCounter.Api/ src/AdjacentPairCounter.Api/
COPY src/AdjacentPairCounter.Application/ src/AdjacentPairCounter.Application/
COPY src/AdjacentPairCounter.Domain/ src/AdjacentPairCounter.Domain/

RUN dotnet publish src/AdjacentPairCounter.Api/AdjacentPairCounter.Api.csproj \
    -c Release -o /app/publish --no-restore

# Runtime stage: ASP.NET runtime only, no SDK/compilers
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "AdjacentPairCounter.Api.dll"]
