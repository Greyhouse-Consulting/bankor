FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app
EXPOSE 30000

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Bancor.SiloHost/Bancor.SiloHost.csproj src/Bancor.SiloHost/
COPY src/Bankor.Core.Grains/Bancor.Core.Grains.csproj src/Bankor.Core.Grains/
COPY src/Bancor.Core/Bancor.Core.csproj src/Bancor.Core/
COPY src/Bancor.Core.Grains.Interfaces/Bancor.Core.Grains.Interfaces.csproj src/Bancor.Core.Grains.Interfaces/
COPY src/Bancor.Infrastructure/Bancor.Infrastructure.csproj src/Bancor.Infrastructure/
RUN dotnet restore src/Bancor.SiloHost/Bancor.SiloHost.csproj
COPY . .
WORKDIR /src/src/Bancor.SiloHost
RUN dotnet build Bancor.SiloHost.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Bancor.SiloHost.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Bancor.SiloHost.dll"]
