FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Trekify.API.csproj", "."]
RUN dotnet restore "./Trekify.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Trekify.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trekify.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy data and images from parent directory
COPY ../data ./data
COPY ../Images ./Images

ENTRYPOINT ["dotnet", "Trekify.API.dll"]