FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WechatServer/WechatServer.csproj", "WechatServer/"]
RUN dotnet restore "WechatServer/WechatServer.csproj"
COPY . .
WORKDIR "/src/WechatServer"
RUN dotnet build "WechatServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WechatServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WechatServer.dll"]
