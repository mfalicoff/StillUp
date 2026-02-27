# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Restore dependencies first for better layer caching
COPY ["StillUp.ApiService/StillUp.ApiService.csproj", "StillUp.ApiService/"]
COPY ["StillUp.ServiceDefaults/StillUp.ServiceDefaults.csproj", "StillUp.ServiceDefaults/"]
RUN dotnet restore "StillUp.ApiService/StillUp.ApiService.csproj"

COPY . .
RUN dotnet publish "StillUp.ApiService/StillUp.ApiService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5370
ENV ASPNETCORE_URLS=http://+:5370

ENTRYPOINT ["dotnet", "StillUp.ApiService.dll"]
