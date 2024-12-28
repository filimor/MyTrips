FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MyTrips.Presentation/MyTrips.Presentation.csproj", "./MyTrips.Presentation/"]
COPY ["src/MyTrips.Application/MyTrips.Application.csproj", "./MyTrips.Application/"]
COPY ["src/MyTrips.Domain/MyTrips.Domain.csproj", "./MyTrips.Domain/"]
COPY ["src/MyTrips.Infrastructure/MyTrips.Infrastructure.csproj", "./MyTrips.Infrastructure/"]
COPY ["src/MyTrips.CrossCutting/MyTrips.CrossCutting.csproj", "./MyTrips.CrossCutting/"]
RUN dotnet restore "MyTrips.Presentation/MyTrips.Presentation.csproj"
COPY src/. /src
WORKDIR "/src/MyTrips.Presentation"
RUN dotnet build "./MyTrips.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTrips.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTrips.Presentation.dll"]