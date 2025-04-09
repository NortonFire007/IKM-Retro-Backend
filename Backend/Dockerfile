FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IKM-Retro.csproj", "./"]
RUN dotnet restore "./IKM-Retro.csproj"
COPY . .
RUN dotnet build "./IKM-Retro.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./IKM-Retro.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IKM-Retro.dll"]