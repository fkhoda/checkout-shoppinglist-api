FROM microsoft/aspnetcore-build:1.1.2
WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish --output /out/ --configuration Release

ENV ASPNETCORE_URLS http://*:5000
WORKDIR /out
ENTRYPOINT ["dotnet", "ShoppingListService.Infrastructure.WebApi.dll"]
