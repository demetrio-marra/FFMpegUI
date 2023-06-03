#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Install FFmpeg
RUN apt-get update \
    && apt-get install -y ffmpeg \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["FFMpegUI/FFMpegUI.csproj", "FFMpegUI/"]
RUN dotnet restore "FFMpegUI/FFMpegUI.csproj"
COPY . .
WORKDIR "/src/FFMpegUI"
RUN dotnet build "FFMpegUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FFMpegUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FFMpegUI.dll"]