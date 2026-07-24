# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["StudentManagement.sln", "./"]
COPY ["src/StudentManagement.API/StudentManagement.API.csproj", "src/StudentManagement.API/"]
COPY ["src/StudentManagement.Application/StudentManagement.Application.csproj", "src/StudentManagement.Application/"]
COPY ["src/StudentManagement.Domain/StudentManagement.Domain.csproj", "src/StudentManagement.Domain/"]
COPY ["src/StudentManagement.Infrastructure/StudentManagement.Infrastructure.csproj", "src/StudentManagement.Infrastructure/"]
COPY ["src/StudentManagement.Persistence/StudentManagement.Persistence.csproj", "src/StudentManagement.Persistence/"]
COPY ["tests/StudentManagement.Tests/StudentManagement.Tests.csproj", "tests/StudentManagement.Tests/"]

RUN dotnet restore "StudentManagement.sln"

# Copy remaining source code and publish
COPY . .
WORKDIR "/src/src/StudentManagement.API"
RUN dotnet build "StudentManagement.API.csproj" -c Release -o /app/build
RUN dotnet publish "StudentManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StudentManagement.API.dll"]
