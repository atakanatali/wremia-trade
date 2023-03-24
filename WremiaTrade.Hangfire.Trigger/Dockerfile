FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WremiaTrade.Hangfire.Trigger/WremiaTrade.Hangfire.Trigger.csproj", "WremiaTrade.Hangfire.Trigger/"]
RUN dotnet restore "WremiaTrade.Hangfire.Trigger/WremiaTrade.Hangfire.Trigger.csproj"
COPY . .
WORKDIR "/src/WremiaTrade.Hangfire.Trigger"
RUN dotnet build "WremiaTrade.Hangfire.Trigger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WremiaTrade.Hangfire.Trigger.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WremiaTrade.Hangfire.Trigger.dll"]
