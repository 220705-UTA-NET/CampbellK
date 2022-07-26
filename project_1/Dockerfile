# https://stackoverflow.com/questions/47103570/asp-net-core-2-0-multiple-projects-solution-docker-file

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
# COPY *.sln .
COPY . .
# COPY ./Api/*.csproj ./Api
# COPY ./Data/*.csproj ./Data
# COPY ./Console/*.csproj ./Console
# COPY . .
RUN dotnet restore

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /out

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /Api
COPY --from=build /app/out .

EXPOSE 7106

ENTRYPOINT ["dotnet", "api.dll"]
