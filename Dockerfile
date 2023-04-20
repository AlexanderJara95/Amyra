FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Amyra.csproj", "./"]
RUN dotnet restore "./Amyra.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Amyra.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Amyra.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Amyra.dll