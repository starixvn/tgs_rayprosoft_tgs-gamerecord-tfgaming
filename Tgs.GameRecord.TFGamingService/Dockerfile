#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.TFGamingService/Tgs.GameRecord.TFGamingService.csproj", "Tgs.GameRecord.TFGamingService/"]
COPY ["Tgs.GameRecord.TFGamingService.Core/Tgs.GameRecord.TFGamingService.Core.csproj", "Tgs.GameRecord.TFGamingService.Core/"]
RUN dotnet restore "Tgs.GameRecord.TFGamingService/Tgs.GameRecord.TFGamingService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.TFGamingService"
RUN dotnet build "Tgs.GameRecord.TFGamingService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.TFGamingService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.TFGamingService.dll"]