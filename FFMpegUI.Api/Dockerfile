#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update && apt-get install -y ffmpeg # also in build image to let it work also in debug
WORKDIR /src
COPY ["FFMpegUI.Api", "FFMpegUI.Api/"]
COPY ["FFMpegUI.Infrastructure", "FFMpegUI.Infrastructure/"]
COPY ["FFMpegUI.Persistence", "FFMpegUI.Persistence/"]
COPY ["FFMpegUI.Persistence.Definitions", "FFMpegUI.Persistence.Definitions/"]
COPY ["FFMpegUI", "FFMpegUI/"]
COPY ["FFMpegUI.Services", "FFMpegUI.Services/"]
COPY ["FFMpegUI.Resilience", "FFMpegUI.Resilience/"]
RUN dotnet restore "FFMpegUI.Api/FFMpegUI.Api.csproj"
WORKDIR "/src/FFMpegUI.Api"
RUN dotnet build "FFMpegUI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FFMpegUI.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# when debugging from Visual Studio the steps beyond this line are not executed.
# Put any additional required package in the previous image

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
RUN apt-get update && apt-get install -y ffmpeg
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FFMpegUI.Api.dll"]