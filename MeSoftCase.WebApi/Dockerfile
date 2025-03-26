FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 9090
EXPOSE 9091

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MeSoftCase.WebApi/MeSoftCase.WebApi.csproj", "MeSoftCase.WebApi/"]
COPY ["MeSoftCase.WebApi.Application/MeSoftCase.WebApi.Application.csproj", "MeSoftCase.WebApi.Application/"]
COPY ["MeSoftCase.WebApi.Infrastructure/MeSoftCase.WebApi.Infrastructure.csproj", "MeSoftCase.WebApi.Infrastructure/"]
COPY ["MeSoftCase.WebApi.Domain/MeSoftCase.WebApi.Domain.csproj", "MeSoftCase.WebApi.Domain/"]
RUN dotnet restore "MeSoftCase.WebApi/MeSoftCase.WebApi.csproj"
COPY . .
WORKDIR "/src/MeSoftCase.WebApi"
RUN dotnet build "MeSoftCase.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MeSoftCase.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeSoftCase.WebApi.dll"]
