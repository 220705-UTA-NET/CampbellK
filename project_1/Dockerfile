# https://stackoverflow.com/questions/47103570/asp-net-core-2-0-multiple-projects-solution-docker-file

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore
RUN dotnet publish -c release -o out

# final stage/image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0
#WORKDIR /app
#COPY --from=build /app/out .

EXPOSE 7106

ENTRYPOINT ["dotnet", "/app/Api/api.dll"]
