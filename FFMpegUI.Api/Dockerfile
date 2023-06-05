#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Update package lists and install FFmpeg manually
RUN apt update && apt install -y ffmpeg


WORKDIR /src
COPY ["FFMpegUI.Api/FFMpegUI.Api.csproj", "FFMpegUI.Api/"]
RUN dotnet restore "FFMpegUI.Api/FFMpegUI.Api.csproj"
COPY . .
WORKDIR "/src/FFMpegUI.Api"
RUN dotnet build "FFMpegUI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FFMpegUI.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FFMpegUI.Api.dll"]