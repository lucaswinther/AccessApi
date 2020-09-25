#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["src/Access.Auth.Service.Host/Access.Auth.Service.Host.csproj", "src/Access.Auth.Service.Host/"]
COPY ["src/Access.Auth.Service.Domain/Access.Auth.Service.Domain.csproj", "src/Access.Auth.Service.Domain/"]
COPY ["src/Access.Auth.Service.Business/Access.Auth.Service.Business.csproj", "src/Access.Auth.Service.Business/"]
COPY ["src/Access.Auth.Service.Infra/Access.Auth.Service.Infra.csproj", "src/Access.Auth.Service.Infra/"]
RUN dotnet restore "src/Access.Auth.Service.Host/Access.Auth.Service.Host.csproj"
COPY . .
WORKDIR "/src/src/Access.Auth.Service.Host"
RUN dotnet build "Access.Auth.Service.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Access.Auth.Service.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Access.Auth.Service.Host.dll"]