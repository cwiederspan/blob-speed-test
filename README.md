

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