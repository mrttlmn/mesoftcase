FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MeSoftCase.UI/MeSoftCase.UI.csproj", "MeSoftCase.UI/"]
RUN dotnet restore "MeSoftCase.UI/MeSoftCase.UI.csproj"
COPY . .
WORKDIR "/src/MeSoftCase.UI"
RUN dotnet build "MeSoftCase.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MeSoftCase.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeSoftCase.UI.dll"]
