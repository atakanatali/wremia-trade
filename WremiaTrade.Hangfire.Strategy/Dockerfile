FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WremiaTrade.Hangfire.Strategy/WremiaTrade.Hangfire.Strategy.csproj", "WremiaTrade.Hangfire.Strategy/"]
RUN dotnet restore "WremiaTrade.Hangfire.Strategy/WremiaTrade.Hangfire.Strategy.csproj"
COPY . .
WORKDIR "/src/WremiaTrade.Hangfire.Strategy"
RUN dotnet build "WremiaTrade.Hangfire.Strategy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WremiaTrade.Hangfire.Strategy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WremiaTrade.Hangfire.Strategy.dll"]
