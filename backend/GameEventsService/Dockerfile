FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GameEventsService/GameEventsService.csproj", "GameEventsService/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
RUN dotnet restore "GameEventsService/GameEventsService.csproj"
COPY . .
WORKDIR "/src/GameEventsService"
RUN dotnet build "GameEventsService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GameEventsService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GameEventsService.dll"]
