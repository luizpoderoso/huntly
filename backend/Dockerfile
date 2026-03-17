FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Huntly.Api/Huntly.Api.csproj", "src/Huntly.Api/"]
COPY ["src/Huntly.Core/Huntly.Core.csproj", "src/Huntly.Core/"]
COPY ["src/Huntly.Application/Huntly.Application.csproj", "src/Huntly.Application/"]
COPY ["src/Huntly.Infra/Huntly.Infra.csproj", "src/Huntly.Infra/"]

RUN dotnet restore "src/Huntly.Api/Huntly.Api.csproj"

COPY . .

RUN dotnet publish "src/Huntly.Api/Huntly.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Huntly.Api.dll"]