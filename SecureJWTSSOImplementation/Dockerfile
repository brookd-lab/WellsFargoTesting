FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file first (ensure correct path)
COPY ["SecureJWTSSOImplementation.csproj", "./"]
RUN dotnet restore "SecureJWTSSOImplementation.csproj"

# Copy all source files
COPY . .

# Build with verbose output to see exact errors
RUN dotnet build "SecureJWTSSOImplementation.csproj" -c $BUILD_CONFIGURATION -o /app/build --verbosity normal

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SecureJWTSSOImplementation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecureJWTSSOImplementation.dll"]
