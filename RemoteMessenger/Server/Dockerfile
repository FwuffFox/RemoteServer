FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RemoteMessenger/Server/RemoteMessenger.Server.csproj", "RemoteMessenger/Server/"]
COPY ["RemoteMessenger/Shared/RemoteMessenger.Shared.csproj", "RemoteMessenger/Shared/"]
RUN dotnet restore "RemoteMessenger/Server/RemoteMessenger.Server.csproj"
COPY . .
WORKDIR "/src/RemoteMessenger/Server"
RUN dotnet build "RemoteMessenger.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RemoteMessenger.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RemoteMessenger.Server.dll"]
