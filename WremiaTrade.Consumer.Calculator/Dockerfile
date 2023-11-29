FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WremiaTrade.Hangfire.Calculator/WremiaTrade.Hangfire.Calculator.csproj", "WremiaTrade.Hangfire.Calculator/"]
RUN dotnet restore "WremiaTrade.Hangfire.Calculator/WremiaTrade.Hangfire.Calculator.csproj"
COPY . .
WORKDIR "/src/WremiaTrade.Hangfire.Calculator"
RUN dotnet build "WremiaTrade.Hangfire.Calculator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WremiaTrade.Hangfire.Calculator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WremiaTrade.Hangfire.Calculator.dll"]
