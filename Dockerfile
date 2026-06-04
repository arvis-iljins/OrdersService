FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrdersMicroservice.API/OrdersMicroservice.API.csproj", "OrdersMicroservice.API/"]
COPY ["BusinessLogicLayer/BusinessLogicLayer.csproj", "BusinessLogicLayer/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]

RUN dotnet restore "OrdersMicroservice.API/OrdersMicroservice.API.csproj"

COPY . .

WORKDIR "/src/OrdersMicroservice.API"
RUN dotnet build "OrdersMicroservice.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrdersMicroservice.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrdersMicroservice.API.dll"]
