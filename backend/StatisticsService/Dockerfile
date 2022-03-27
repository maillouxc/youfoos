FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StatisticsService/StatisticsService.csproj", "StatisticsService/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
RUN dotnet restore "StatisticsService/StatisticsService.csproj"
COPY . .
WORKDIR "/src/StatisticsService"
RUN dotnet build "StatisticsService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StatisticsService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "StatisticsService.dll"]
