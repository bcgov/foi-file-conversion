#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
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