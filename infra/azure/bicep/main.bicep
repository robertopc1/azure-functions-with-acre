targetScope = 'subscription'

@description('Optional. Azure main location to which the resources are to be deployed -defaults to the location of the current deployment')
param location string = deployment().location

@description('Optional. The tags to be assigned to the created resources.')
param tags object = {}

@description('Required. Caching Pattern')
@allowed([
    'Ingest'
    'Write-Behind'
])
param cachingPattern string = 'Ingest'

var applicationName = 'cachingpatterns'

@description('Required. SQL Admin Password')
@secure()
param sqlAdminPassword string

var defaultTags = union({
  application: applicationName
}, tags)

var appResourceGroupName = 'rg-${applicationName}'
var sharedResourceGroupName = 'rg-shared-${applicationName}'

// Create resource groups
resource appResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: appResourceGroupName
  location: location
  tags: defaultTags
}

resource sharedResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: sharedResourceGroupName
  location: location
  tags: defaultTags
}

// Create shared resources
module shared './shared/shared.bicep' = {
  name: 'sharedresources-Deployment'
  scope: resourceGroup(sharedResourceGroup.name)
  params: {
    location: location
    applicationName: applicationName
    tags: defaultTags
  }
}

// Get Key Vault information
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: shared.outputs.keyVaultName
  scope: resourceGroup(subscription().id, sharedResourceGroupName )
}

//Create SQL Resource
module sql 'sql.bicep' = {
  dependsOn:[
    kv
  ]
  scope: resourceGroup(appResourceGroup.name)
  name: 'sql-Deployment'
  params: {
    location: location
    tags: tags
    applicationName: applicationName
    keyVaultName: shared.outputs.keyVaultName
    adminPassword: sqlAdminPassword
  }
}

//Create Redis resource
module redis 'redis.bicep' = {
  dependsOn:[
    kv
  ]
  scope: resourceGroup(appResourceGroup.name)
  name: 'redis-Deployment'
  params: {
    location: location
    tags: defaultTags
    keyVaultName: shared.outputs.keyVaultName
    applicationName: applicationName
  }
}

//Create App Service resource
module appService 'app.bicep' = {
    dependsOn:[
        kv
        redis
        sql
        shared
      ]
      scope: resourceGroup(appResourceGroup.name)
      name: 'appService-Deployment'
      params: {
        location: location
        tags: tags
        applicationName: applicationName
        appiConnectionString: shared.outputs.appInsightsConnectionString
        redisHostName: kv.getSecret('redis1HostName')
        redisPassword: kv.getSecret('redis1Password')
        azureSQLConnectionString: kv.getSecret('azureSqlConnectionString')
      }
}

//Create Function Apps
module functionApps 'func.bicep' = {
  dependsOn: [
   sql
   redis
   appService
  ]
  scope: resourceGroup(appResourceGroup.name)
  name: 'functionApps-Deployment'
  params: {
    location: location
    tags: tags
    applicationName: applicationName
    appiConnectionString: shared.outputs.appInsightsConnectionString
    cachingPattern: cachingPattern
  }
}

output appResourceGroupName string = appResourceGroup.name
output sharedResourceGroupName string = sharedResourceGroup.name
