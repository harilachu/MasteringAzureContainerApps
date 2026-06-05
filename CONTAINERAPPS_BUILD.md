# Container Image Build Commands

## 🔧 1. Create the image and publish to Azure registry (ACR)

`Note 1:` ❗ Change the version number before building

`Note 2:` ❌ Close solution file if opened from Visual studio

`Note 3:` ℹ️ Run the below commands from Solution file path

### YARP Gateway Service
```azurepowershell
az acr build --registry containerreg --file ./Microservices/YARP/YarpGatewayService/DockerFile --image yarpgatewayservice:v1 .
```
### Employee App
```azurepowershell
az acr build --registry containerreg --file ./Microservices/Employees/ERP.Employees.API/DockerFile --image employeeapp:v1 .
```
### Review App
```azurepowershell
az acr build --registry containerreg --file ./Microservices/PerformanceReviews/ERP.PerfReview.API/DockerFile --image reviewapp:v1 .
```
---   
<br/>

## 📦 2. Deploy Images to Container Apps
### ***YARP Container*** (yarp-container-app)
```azurepowershell
az containerapp update --name yarp-container-app --resource-group masteraca --image containerreg.azurecr.io/yarpgatewayservice:v1
```
### ***Employee Container*** (employee-container-app)
```azurepowershell
az containerapp update --name employee-container-app --resource-group masteraca --image containerreg.azurecr.io/employeeapp:v1
```
### ***Review Container*** (review-container-app)
```azurepowershell
az containerapp update --name review-container-app --resource-group masteraca --image containerreg.azurecr.io/reviewapp:v1
```
---
  <br/>

> `Note :`  ❗Use Either (Option 2 - Image) or (Option 3 - Yaml) to update the Container Apps with new image  
  

## 📩 3.a Download Yaml file of Container Apps
ℹ️ Run the below commands from the Yaml file location
### ***YARP Container*** (yarp-container-app)
```azurepowershell
az containerapp show --name yarp-container-app --resource-group masteraca --output yaml > yarp-container-app.yaml
```
### ***Employee Container*** (employee-container-app)
```azurepowershell
az containerapp show --name employee-container-app --resource-group masteraca --output yaml > employee-container-app.yaml
```
### ***Review Container*** (review-container-app)
```azurepowershell
az containerapp show --name review-container-app --resource-group masteraca --output yaml > review-container-app.yaml
```
---
## 🔼 3.b Deploy updated Yaml
ℹ️ Run the below commands from the Yaml file location
### ***YARP Container*** (yarp-container-app)
```azurepowershell
az containerapp update --name yarp-container-app --resource-group masteraca --yaml ./yarp-container-app.yaml
```
### ***Employee Container*** (employee-container-app)
```azurepowershell
az containerapp update --name employee-container-app --resource-group masteraca --yaml ./employee-container-app.yaml
```
### ***Review Container*** (review-container-app)
```azurepowershell
az containerapp update --name review-container-app --resource-group masteraca --yaml ./review-container-app.yaml
```
---