FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["PaymentSystem/PaymentSystem.csproj", "PaymentSystem/"]
RUN dotnet restore "PaymentSystem/PaymentSystem.csproj"
COPY . .
WORKDIR "/src/PaymentSystem"
RUN dotnet build "PaymentSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentSystem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentSystem.dll"]
