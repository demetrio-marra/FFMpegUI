#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["FFMpegUI.Mvc", "FFMpegUI.Mvc/"]
COPY ["FFMpegUI.Infrastructure", "FFMpegUI.Infrastructure/"]
COPY ["FFMpegUI.Persistence", "FFMpegUI.Persistence/"]
COPY ["FFMpegUI.Persistence.Definitions", "FFMpegUI.Persistence.Definitions/"]
COPY ["FFMpegUI", "FFMpegUI/"]
COPY ["FFMpegUI.Services", "FFMpegUI.Services/"]
RUN dotnet restore "FFMpegUI.Mvc/FFMpegUI.Mvc.csproj"
WORKDIR "/src/FFMpegUI.Mvc"
RUN dotnet build "FFMpegUI.Mvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FFMpegUI.Mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
RUN apt-get install -y netcat
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FFMpegUI.Mvc.dll"]

