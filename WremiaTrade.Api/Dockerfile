FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WremiaTrade.Api/WremiaTrade.Api.csproj", "WremiaTrade.Api/"]
RUN dotnet restore "WremiaTrade.Api/WremiaTrade.Api.csproj"
COPY . .
WORKDIR "/src/WremiaTrade.Api"
RUN dotnet build "WremiaTrade.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WremiaTrade.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WremiaTrade.Api.dll"]
