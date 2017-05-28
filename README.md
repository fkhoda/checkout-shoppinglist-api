# Checkout Shopping List API


## Prerequisites

- ASP.NET Core 1.1

## Build

```
$ ./build.ps1
```

## Run

```
$ ./run.ps1
```

## Test

```
$ ./tests.ps1
```

## Docker

### Build Image

```
$ dotnet restore
$ dotnet publish -o build -c Release
$ docker build -t fkhoda/checkout-shoppinglist-api .
```

### Run with Docker Compose

```
$ docker-compose up
```

### Access

- API: <http://{docker-machine-ip}:8080>
- Elasticsearch: <http://{docker-machine-ip}:9200>
- Kibana: <http://{docker-machine-ip}:5601>
    - Username: elastic
    - Password: changeme
    
## Kubernetes

### Push Docker Image to Registry

```
$ docker push fkhoda/checkout-shoppinglist-api
```

### Deploy to Minikube Cluster

Deploy ELK for monitoring (Optional)

```
$ git clone https://github.com/fkhoda/elk-kubernetes.git
$ cd elk-kubernetes
$ kubectl create -f . -R --namespace=default
```

Deploy Shopping List API

```
$ kubectl create -f ./kubernetes -R --namespace=default
```

### Access

- API: <http://{minikube-ip}:30080>
- Elasticsearch: <http://{minikube-ip}:30920>
- Kibana: <http://{minikube-ip}:31601>
    - Username: elastic
    - Password: changeme

## Architecture
- Onion Architecture
- Actor Model with in-memory Event Sourcing and Snapshotting using Proto Actor (<http://proto.actor>)

![](https://raw.githubusercontent.com/fkhoda/checkout-shoppinglist-api/master/docs/actor-model.png)

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