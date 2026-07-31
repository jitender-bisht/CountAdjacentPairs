# Provisioning Commands

Every CLI command used to create or configure an Azure or Kubernetes resource in this project, in the order they were run. This is a reference for reproducing the setup — see [README.md](../README.md) for the explained, step-by-step walkthrough, and [LLD.md](LLD.md) for exact resource specs and rationale.

Diagnostic/exploratory commands (`az vm list-skus`, `az vm list-usage`, `az quota list`, failed size attempts while working around regional capacity restrictions) are intentionally omitted — only commands that actually created or configured something are listed here.

## Authentication

```bash
az login
```

## Resource Group

```bash
az group create --name rg-apc-learn --location eastus2
```

## Resource provider registration

One-time per subscription — required before creating resources of these types.

```bash
az provider register --namespace Microsoft.ContainerRegistry
az provider register --namespace Microsoft.ContainerService
az provider register --namespace Microsoft.Compute
az provider register --namespace Microsoft.Network
az provider register --namespace Microsoft.KeyVault
az provider register --namespace Microsoft.OperationalInsights
az provider register --namespace Microsoft.Insights
az provider register --namespace Microsoft.ManagedIdentity
az provider register --namespace Microsoft.OperationsManagement
```

## Azure Container Registry

```bash
az acr create --resource-group rg-apc-learn --name acrapcmlsryg --sku Basic --admin-enabled false
```

## Azure AD App Registration + OIDC federation (for GitHub Actions)

```bash
az ad app create --display-name "gh-apc-cd"
az ad sp create --id <gh-apc-cd-app-id>

az ad app federated-credential create --id <gh-apc-cd-app-id> --parameters '{
  "name": "gh-apc-main-branch",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:jitender-bisht/CountAdjacentPairs:ref:refs/heads/main",
  "audiences": ["api://AzureADTokenExchange"]
}'
# Later updated to match GitHub's immutable-ID subject format - see the
# "Federated credentials" section further down.

az role assignment create --assignee <gh-apc-cd-app-id> --role AcrPush --scope <acr-resource-id>
```

## Networking (VM phase — Central US, after East US 2 hit capacity limits)

```bash
az network vnet create --resource-group rg-apc-learn --name vnet-apc --location centralus \
  --address-prefix 10.10.0.0/16 --subnet-name snet-apc-vm --subnet-prefix 10.10.1.0/24

az network nsg create --resource-group rg-apc-learn --name nsg-apc-vm --location centralus

az network nsg rule create --resource-group rg-apc-learn --nsg-name nsg-apc-vm \
  --name Allow-SSH-MyIP --priority 100 --direction Inbound --access Allow --protocol Tcp \
  --source-address-prefixes <my-ip>/32 --destination-port-ranges 22

az network nsg rule create --resource-group rg-apc-learn --nsg-name nsg-apc-vm \
  --name Allow-App-8080 --priority 110 --direction Inbound --access Allow --protocol Tcp \
  --source-address-prefixes '*' --destination-port-ranges 8080
```

## Virtual Machine (historical — superseded by AKS)

```bash
az vm create \
  --resource-group rg-apc-learn --name vm-apc-host --location centralus \
  --image Ubuntu2404 --size Standard_B2s_v2 \
  --vnet-name vnet-apc --subnet snet-apc-vm --nsg nsg-apc-vm \
  --admin-username azureuser --ssh-key-values ~/.ssh/id_rsa.pub \
  --assign-identity --custom-data cloud-init.yaml

az role assignment create --assignee <vm-managed-identity-principal-id> --role AcrPull --scope <acr-resource-id>
```

## AKS Cluster

```bash
az aks create \
  --resource-group rg-apc-learn --name aks-apc --location centralus \
  --node-count 1 --node-vm-size Standard_B2s_v2 \
  --enable-managed-identity --attach-acr acrapcmlsryg \
  --generate-ssh-keys --enable-oidc-issuer --enable-workload-identity

az aks get-credentials --resource-group rg-apc-learn --name aks-apc --overwrite-existing
az aks install-cli

az role assignment create --assignee <gh-apc-cd-app-id> \
  --role "Azure Kubernetes Service Cluster Admin Role" --scope <aks-resource-id>
```

## Federated credentials (one per GitHub Environment)

Each GitHub Environment produces a distinct OIDC token subject, so each needs its own federated credential.

```bash
az ad app federated-credential create --id <gh-apc-cd-app-id> --parameters '{
  "name": "gh-apc-env-dev",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:jitender-bisht@<ownerId>/CountAdjacentPairs@<repoId>:environment:dev",
  "audiences": ["api://AzureADTokenExchange"]
}'

az ad app federated-credential create --id <gh-apc-cd-app-id> --parameters '{
  "name": "gh-apc-env-production",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:jitender-bisht@<ownerId>/CountAdjacentPairs@<repoId>:environment:production",
  "audiences": ["api://AzureADTokenExchange"]
}'
```

## Kubernetes resources (`kubectl`, applied to `aks-apc`)

```bash
kubectl apply -f k8s/namespaces.yaml

kubectl apply -f k8s/dev/configmap.yaml
kubectl apply -f k8s/dev/deployment.yaml
kubectl apply -f k8s/dev/service.yaml
kubectl apply -f k8s/dev/hpa.yaml
kubectl apply -f k8s/dev/ingress.yaml

kubectl apply -f k8s/prod/configmap.yaml
kubectl apply -f k8s/prod/deployment.yaml
kubectl apply -f k8s/prod/service.yaml
kubectl apply -f k8s/prod/hpa.yaml
kubectl apply -f k8s/prod/ingress.yaml
```

### NGINX Ingress Controller (cluster-wide, one-time)

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.15.1/deploy/static/provider/cloud/deploy.yaml
```
