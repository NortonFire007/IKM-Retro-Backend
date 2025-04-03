FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80

ENV ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=ikmdb;Username=ikmusername;Password=ikmpassword;"

ENTRYPOINT ["sh", "-c", "dotnet ef database update && dotnet IKM-Retro.dll"]