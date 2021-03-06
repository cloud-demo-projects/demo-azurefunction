[![Build Status](https://dev.azure.com/bahrinipun/demo-azurefunction/_apis/build/status/azurefunction-cicd?branchName=main)](https://dev.azure.com/bahrinipun/demo-azurefunction/_build/latest?definitionId=53&branchName=main)

# Develop and Deploy C# based Azure Function with Azure DevOps
This is a sample project for C# based Azure function which intends to demonstrate-
- Dependency Injection
- JSON request parsing and desialization
- Http Request validation through fluent validation
- Service calls using Httpclient
- ARM REST API call through function's system assigned managed identity
- KeyVault interaction through system assigned managed identity
- Timer trigger based custom health check
- Swagger Documentation
- Azure DevOps interaction through PAT token
- Azure DevOps pipeline as CICD 

## Prerequisites
Prerequisites are listed down for local development and CICD.

### Local Development
- VS Code
- .Net Core SDK
- Azure CLI
- Azure Functions Core Tools
- Extension (C#, Azure Functions)

### CI/CD
- Azure Devops Organization
- Azure Subscription
- Azure FunctionApp
- Service Principle with appropriate permissions 
