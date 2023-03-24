FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WremiaTrade.Hangfire.Trade/WremiaTrade.Hangfire.Trade.csproj", "WremiaTrade.Hangfire.Trade/"]
RUN dotnet restore "WremiaTrade.Hangfire.Trade/WremiaTrade.Hangfire.Trade.csproj"
COPY . .
WORKDIR "/src/WremiaTrade.Hangfire.Trade"
RUN dotnet build "WremiaTrade.Hangfire.Trade.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WremiaTrade.Hangfire.Trade.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WremiaTrade.Hangfire.Trade.dll"]
