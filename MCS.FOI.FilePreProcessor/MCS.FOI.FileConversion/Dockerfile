#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
#RUN apt-get update -y && apt-get install libfontconfig -y 

RUN apt-get update && apt-get -y install xvfb && apt-get -y install fontconfig && apt-get -y install libssl1.1 && apt-get -y install libx11-dev libx11-xcb-dev libxcb-icccm4-dev libxcb-image0-dev libxcb-keysyms1-dev libxcb-randr0-dev libxcb-render-util0-dev libxcb-render0-dev libxcb-shm0-dev libxcb-util0-dev libxcb-xfixes0-dev libxcb-xkb-dev libxcb1-dev libxfixes-dev libxrandr-dev libxrender-dev

ENV LANG="en_US.UTF-8" 
WORKDIR /app
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MCS.FOI.FileConversion/MCS.FOI.FileConversion.csproj", "MCS.FOI.FileConversion/"]
RUN dotnet restore "MCS.FOI.FileConversion/MCS.FOI.FileConversion.csproj"
COPY . .
WORKDIR "/src/MCS.FOI.FileConversion"
RUN dotnet build "MCS.FOI.FileConversion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MCS.FOI.FileConversion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MCS.FOI.FileConversion.dll"]