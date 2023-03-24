FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WremiaTrade.Hangfire.Notification/WremiaTrade.Hangfire.Notification.csproj", "WremiaTrade.Hangfire.Notification/"]
RUN dotnet restore "WremiaTrade.Hangfire.Notification/WremiaTrade.Hangfire.Notification.csproj"
COPY . .
WORKDIR "/src/WremiaTrade.Hangfire.Notification"
RUN dotnet build "WremiaTrade.Hangfire.Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WremiaTrade.Hangfire.Notification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WremiaTrade.Hangfire.Notification.dll"]
