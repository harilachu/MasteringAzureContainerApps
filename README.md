# 🌐 MasteringAzureContainerApps 
This repository comprises of microservice projects used for Udemy course learning Azure Container Apps, which can be hosted in any microservice orchestration environment.

## 🥡 CosmosDB 
`CosmosDB Emulator` is used for local development and testing, and `Azure CosmosDB` is used for production deployment. The microservices are built using .NET 10, and the container images are built using .NET 10 SDK image as the base image.  
[Azure Cosmos DB Emulator Installer](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-develop-emulator?tabs=windows%2Ccsharp&pivots=api-nosql#install-the-emulator) 📦

**CosmosDB json** files can be found in the [`CosmosDB`](https://github.com/harilachu/MasteringAzureContainerApps/tree/main/CosmosDB) 📁 folder.  

## 🏛️ Architecture 
**Clean Architecture** is followed for the microservices, and the solution structure is organized in a way to separate concerns and promote maintainability.  
**CQRS pattern** is implemented for the microservices, and `Synaptrix` library is used for handling commands and queries.

### 📦 Synaptrix Package
Concordia is renamed to Synaptrix lately.  
Package Github Repository: [Synaptrix / Concordia](https://github.com/mrdevrobot/Concordia)

## 🕸 Microservices built with .NET 10  
 - YARP Gateway Service
- Employee App
- Review App


### 📥 Startup Projects
Set Startup projects in Visual Studio to the 3 microservices to run them together for local development and testing.
```
1. YarpGatewayService
2. ERP.Employees.API
3. ERP.PerfReview.API
```

Run these projects from Visual Studio or using the terminal with `dotnet run` command to start the microservices locally for development and testing.

---

##  📫 Postman files for local testing ##

The postman collection can be found in the [`postman`](https://github.com/harilachu/MasteringAzureContainerApps/tree/main/postman/collections) 📁 folder.  
Import this collection into postman to test the APIs locally or after Azure deployments.
The collection contains all the API endpoints and request parameters pre-configured.

---

## 🧑‍💻 Build Container Apps  ##
Container Apps can be built referring to the instructions in [`ContainerApps Build.md`](https://github.com/harilachu/MasteringAzureContainerApps/blob/main/CONTAINERAPPS_BUILD.md) file.

---

## 🎯 Running locally using Dapr  ##

> ℹ️ Dapr Implementation can be found in the branch named `dapr-implementation`, and the instructions mentioned below are for running the microservices locally using Dapr.

`Note :` ❗ **Docker** needs to be installed and running to run the microservices locally using Dapr.

### 1. Checkout branch ###
Check out to the branch named `dapr-implementation` to run the microservices locally using Dapr.
```
git checkout dapr-implementation
```

### 2. Install Dapr CLI ###
> 📄 Refer Dapr documentation for more details on installing Dapr CLI: [Dapr CLI Installation](https://docs.dapr.io/getting-started/install-dapr-cli/)
``` 
winget install Dapr.CLI
```
Dapr will be installed in the System directory `C:\Users\<username>\.dapr\bin` and you can verify the installation by running the following command in the terminal:
```
dapr --h
```
### 3.Start Dapr with docker ###
```
dapr init
```
### 4.Invoke using HTTP ###
Below command will start running the microservices with Dapr sidecar, and you can invoke the microservices using HTTP calls to the Dapr sidecar ports.
> ℹ️ Run these commands in their respective project folders in the terminal to start the microservices with Dapr sidecar.
```
dapr run --app-id employee-app --app-port 5252  --dapr-http-port 3500 -- dotnet run

dapr run --app-id review-app --app-port 5052  --dapr-http-port 3501 -- dotnet run
```
---
