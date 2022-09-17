# IRIS Gatherer for ADF

[![IRIS Gatherer for ADF CI](https://github.com/iris-azure/iris-gatherer-adf/actions/workflows/iris-gatherer-adf-ci.yaml/badge.svg)](https://github.com/iris-azure/iris-gatherer-adf/actions/workflows/iris-gatherer-adf-ci.yaml) [![codecov](https://codecov.io/gh/iris-azure/iris-gatherer-adf/branch/main/graph/badge.svg?token=EX4G4P7FVA)](https://codecov.io/gh/iris-azure/iris-gatherer-adf)

This module is part of the larger IRIS-Azure monitoring suite and is responsible for gathering and importing Data Factory run information.

## Introduction

This module runs as a container-based job and pulls Azure Data Factory run information at a regular interval. This run information is then saved in the database. This job uses Azure Management API to retrieve information from Azure.

Currently, this job saves the information in Azure Cosmos Database (SQL API), but subsequently other database types will be added.

## Getting Started

This module is written in Microsoft .Net 6. Currently, this job has following pre-requisites:

1. **An Service Principal Name (SPN)** with *Read* access to the subscription where the Data Factories reside. The SPN must be created using *secret*. Currently, certificate-based authentication is not supported. Nor does it support managed identity
2. **Cosmos DB in SQL API Mode** to allow saving the run information
3. **Storage Account** to store Data Factory information this job will parse

Currently, this implementation supports Cosmos DB for storing run information and a JSON file in Blob storage as the factory information configuration. Other storage mechanisms are planned for future.

The simplest way to run locally, is to use a *config* file. Create a file named *appsettings.json* and place it in `src/` folder. A sample file, named *appsettings.json.sample*, is provided for your reference. The listing is provided below for your reference.

### Configuring the Job

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    },
    "Console": {
      "DisableColors": true
    }
  },
  "Serializer": {
    "Type": "cosmos",
    "ConnectionString": "<Put the connection string for Cosmos DB>",
    "DatabaseName": "IrisAzure",
    "Container": "adf"
  },
  "JobParams": {
    "TriggerPeriodSeconds": 600,
    "ListLocation": "storage",
    "ListStorageURL": "<Put SAS Token for the pipeline list to monitor>",
    "DaysToKeep": 10
  }
}
```

This module also accepts configurations from environment variables. This is to simplify deployments in containers. The table below provides the description of each of configuration items along with the environment variables:

| JSON Item | Environment Variable Name | Description |
|---|---|---|
| `Logging` > `LogLevel` > `Default` | `IRISAZ_GATHERER_Logging__LogLevel__Default` | The log level for this module. Valid values are `Information`, `Warning` and `Error` |
| `Logging` > `Console` > `DisableColors` | `IRISAZ_GATHERER_Logging__Console__DisableColors` | This option disable colors from the logs. In container deployments it is recommended to disable colors prevent *junk* characters from showing up. The valid values are `true` to disable colors and `false` to show colors |
| `Serializer` > `Type` | `IRISAZ_GATHERER_Serializer__Type` | The serializer to use to store the data. Currently, valid value is `cosmos` |
| `Serializer` > `ConnectionString` | `IRISAZ_GATHERER_Serializer__ConnectionString` | The full connection string for the Cosmos database |
| `Serializer` > `DatabaseName` | `IRISAZ_GATHERER_Serializer__DatabaseName` | The name of the Cosmos DB Database to create or use |
| `Serializer` > `Container` | `IRISAZ_GATHERER_Serializer__Container` | The name of the Cosmos DB container to create or use |
| `JobParams` > `TriggerPeriodSeconds` | `IRISAZ_GATHERER_JobParams__TriggerPeriodSeconds` | The interval at which the data will be refreshed. Care must be taken to not to keep very low value |
| `JobParams` > `ListLocation` | `IRISAZ_GATHERER_JobParams__ListLocation` | The location where the pipeline list is kept. Currently, valid value is `storage` |
| `JobParams` > `ListStorageURL` | `IRISAZ_GATHERER_JobParams__ListStorageURL` | The storage URL along with SAS token to retrieve the pipeline list JSON file |
| `JobParams` > `DaysToKeep` | `IRISAZ_GATHERER_JobParams__DaysToKeep` | The number of days for which the run information will be kept |

### Authentication

This module can accept authentication information from `az` CLI as well as from environment variables. For running locally using `dotnet` CLI, simple ensure that you are logged in using the CLI (by executing `az login`) command.

However, for running the container image or for using SPN, set the following environment variables:

* `AZURE_CLIENT_ID` with the *application id* for the SPN
* `AZURE_TENANT_ID` with the GUID of the Azure Active Directory (AAD)
* `AZURE_CLIENT_SECRET` with the secret of the SPN

### Running the Application Locally

Execute `dotnet run` command to locally run this application. If you want to run unit tests, simply execute `dotnet test` command.

### Building the Docker Image

Execute the following command from the *src/* folder:

```bash
docker build -t iris-azure/adf-gatherer:latest .
```

## Deployment

### Running the Image Locally using Docker

Execute the following command to run this locally using Docker:

```bash
docker pull ghcr.io/iris-azure/iris-gatherer-adf:latest

docker run --env IRISAZ_GATHERER_Logging__LogLevel__Default=Information \
           --env IRISAZ_GATHERER_Logging__Console__DisableColors=true \
           --env IRISAZ_GATHERER_Serializer__Type=cosmos \
           --env IRISAZ_GATHERER_Serializer__ConnectionString="<Cosmos DB Connection String>" \
           --env IRISAZ_GATHERER_Serializer__DatabaseName=IrisAzure \
           --env IRISAZ_GATHERER_Serializer__Container=adf \
           --env IRISAZ_GATHERER_JobParams__TriggerPeriodSeconds=600 \
           --env IRISAZ_GATHERER_JobParams__ListLocation=storage \
           --env IRISAZ_GATHERER_JobParams__ListStorageURL="<URL of the pipeline list with SAS token>" \
           --env IRISAZ_GATHERER_JobParams__DaysToKeep=10 \
           --env AZURE_CLIENT_ID=<Application ID of the APN> \
           --env AZURE_TENANT_ID=<GUID of the AAD tenant> \
           --env AZURE_CLIENT_SECRET=<Secret of the SPN> \
           iris-azure/adf-gatherer:latest
```

### Deploying to Kubernetes

*Documentation to be created*

### Deploying to Azure Container Apps (ACA)

*Documentation to be created*

## Contribute

*Documentation to be created*

## Future Plans

The future roadmap is given below:

Support for following stores for storing run information:

[ ] Postgre SQL
[ ] MySQL
[ ] MariaDb
[ ] Mongo DB
[ ] SQL Server

Support for following stores for storing pipeline information:

[x] Azure BLOB Storage
[ ] Postgre SQL
[ ] MySQL
[ ] MariaDb
[ ] Mongo DB
[ ] SQL Server
