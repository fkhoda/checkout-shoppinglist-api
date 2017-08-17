FROM microsoft/dotnet:2.0-runtime
COPY src/ShoppingListService.DependencyResolution/build /app
COPY src/ShoppingListService.Infrastructure.Actor.Monitoring/build /app
COPY src/ShoppingListService.Infrastructure.Actor.Persistence/build /app
COPY src/ShoppingListService.Infrastructure.Actors/build /app
COPY src/ShoppingListService.WebApi/build /app
WORKDIR /app

EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000

ENTRYPOINT ["dotnet", "ShoppingListService.WebApi.dll"]
