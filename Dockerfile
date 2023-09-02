FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/feynman-technique-backend

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/feynman-technique-backend /app
ENTRYPOINT [ "dotnet", "/app/feynman-technique-backend.dll" ]