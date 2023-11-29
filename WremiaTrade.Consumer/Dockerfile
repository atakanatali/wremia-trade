FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WremiaTrade.Consumer/WremiaTrade.Consumer.csproj", "WremiaTrade.Consumer/"]
RUN dotnet restore "WremiaTrade.Consumer/WremiaTrade.Consumer.csproj"
COPY . .
WORKDIR "/src/WremiaTrade.Consumer"
RUN dotnet build "WremiaTrade.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WremiaTrade.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WremiaTrade.Consumer.dll"]
