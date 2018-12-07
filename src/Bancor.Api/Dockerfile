FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/Bancor.Api/Bancor.Api.csproj", "src/Bancor.Api/"]
COPY ["src/Bancor.Core/Bancor.Core.csproj", "src/Bancor.Core/"]
COPY ["src/Bancor.Infrastructure/Bancor.Infrastructure.csproj", "src/Bancor.Infrastructure/"]
COPY ["src/Bankor.Core.Grains/Bancor.Core.Grains.csproj", "src/Bankor.Core.Grains/"]
COPY ["src/Bancor.Core.Grains.Interfaces/Bancor.Core.Grains.Interfaces.csproj", "src/Bancor.Core.Grains.Interfaces/"]
RUN dotnet restore "src/Bancor.Api/Bancor.Api.csproj"
COPY . .
WORKDIR "/src/src/Bancor.Api"
RUN dotnet build "Bancor.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Bancor.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Bancor.Api.dll"]