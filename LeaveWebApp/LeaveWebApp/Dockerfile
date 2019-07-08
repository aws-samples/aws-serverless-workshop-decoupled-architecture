FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["LeaveWebApp.csproj", "LeaveWebApp/"]
RUN dotnet restore "LeaveWebApp/LeaveWebApp.csproj"
COPY . .
#WORKDIR "/src/LeaveWebApp"
#RUN dotnet build "LeaveWebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LeaveWebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LeaveWebApp.dll"]