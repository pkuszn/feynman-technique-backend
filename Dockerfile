#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/feynman-technique-backend/feynman-technique-backend.csproj", "src/feynman-technique-backend/"]
RUN dotnet restore "src/feynman-technique-backend/feynman-technique-backend.csproj"
COPY . .
WORKDIR "/src/src/feynman-technique-backend"
RUN dotnet build "feynman-technique-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "feynman-technique-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "feynman-technique-backend.dll"]