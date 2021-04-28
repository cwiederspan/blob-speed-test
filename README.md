

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

## Sample File Creation

```cmd

fsutil file createnew small.dat 4000000
fsutil file createnew medium.dat 40000000
fsutil file createnew large.dat 400000000
fsutil file createnew xlarge.dat 4000000000

```
