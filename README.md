# Demo-BC-Managed-Identity

This repository contains a demo Azure Functions app that calls Microsoft Dynamics 365 Business Central by using Microsoft Entra ID and Managed Identity.

## Presentation context

This project is a demo used in a presentation at **Directions EMEA 2026** in **Paris**:

**"OAuth is Secure, Your Secret isn't"**

## What this demo shows

- How to authenticate from Azure-hosted code without storing client secrets
- How to request tokens with `DefaultAzureCredential`
- How to call Business Central APIs using a bearer token
- How to return upstream API response status codes and payloads from the function

## Tech stack

- .NET 10
- Azure Functions (HTTP trigger)
- Azure Identity (`DefaultAzureCredential`)
- Microsoft Dynamics 365 Business Central API

## Run locally

1. Clone this repository.
2. Sign in with Azure CLI or Visual Studio to provide local credentials.
3. Open the solution in Visual Studio.
4. Run the Azure Functions project.
5. Call the `CallBusinessCentral` HTTP endpoint.

## Notes

- This is a demo project for education and presentation purposes.
- Production deployments should use environment-specific configuration, least-privilege access, and proper observability.
