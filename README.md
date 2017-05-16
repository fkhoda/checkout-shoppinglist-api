# Checkout Shopping List API


## Prerequisites

- ASP.NET Core 1.1

## Build

```
./build.ps1
```

## Run

```
./run.ps1
```

## Docker
```
docker build -t checkout/shoppinglist-api .
docker run -it -p 80:5000 checkout/shoppinglist-api
```

## Architecture
- Onion Architecture
- Actor Model with in-memory Event Sourcing and Snapshotting using Proto Actor (<http://proto.actor>)

## Feature Highlights
- Validation using Fluent Validation (<https://github.com/JeremySkinner/FluentValidation>)
- Dead Simple Authentication Middleware
- Native .NET Core Dependency Injection
- Basic Pagination

## Endpoints

### Add Item

Method: POST

URI: /api/v1.0/shoppingLists/{customerId}/items

### Get All Items

Method: GET

URI: /api/v1.0/shoppingLists/{customerId}/items[?pageNumber=1&pageSize=10]

### Get Item

Method: GET

URI: /api/v1.0/shoppingLists/{customerId}/items/{itemName}

### Update Quantity

Method: PUT

URI: api/v1.0/shoppingLists/{customerId}/items/{itemName}

### Delete Item

Method: DELETE

URI: api/v1.0/shoppingLists/{customerId}/items/{itemName}


## Notes

- Some Unit Tests are implemented in the Client Library (<https://github.com/fkhoda/checkout-net-library>)
- The Authentication middleware checks for clients' **Authorization** request header against this value: `sk_test_32b9cb39-1cd6-4f86-b750-7069a133667d`. You may have to specify it manually if you are not using the client library or its Unit Tests.