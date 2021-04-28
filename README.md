# Speed Testing Application

This application can be used to test the performance of Azure blob reads and writes
from an Azure web app in any region, to one or more storage accounts in different 
regions.

## Blob Storage Setup

```bash

LOCATION=westus2

az storage account create \
--name cdwblobs$LOCATION \
--resource-group cdw-blobspeedtesting-20210427 \
--kind StorageV2 \
--location $LOCATION \
--sku Standard_RAGRS \
--access-tier Hot \
--https-only true \
--allow-blob-public-access false

```

## App Service Setup

```bash

LOCATION=westus2

az webapp up \
--name cdw-blobspeedtesting-$LOCATION \
--resource-group cdw-blobspeedtesting-20210427 \
--plan cdw-blobspeedtesting-$LOCATION-plan \
--location $LOCATION \
--runtime "DOTNET|5.0" \
--sku P1V3 \
--os-type Linux

```

## Sample File Creation

```cmd

fsutil file createnew small.dat 4000000
fsutil file createnew medium.dat 40000000
fsutil file createnew large.dat 400000000
fsutil file createnew xlarge.dat 4000000000

```
