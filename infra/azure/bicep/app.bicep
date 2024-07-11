@description('Required. Azure location to which the resources are to be deployed')
param location string

@description('Optional. The tags to be assigned to the created resources.')
param tags object = {}

@description('Required. Application name')
param applicationName string

@description('Optional. App Insights Connection String')
param appiConnectionString string = ''

@description('Required. Redis Host Name')
@secure()
param redisHostName string

@description('Required. Redis Password')
@secure()
param redisPassword string

@description('Required. Azure SQL Connection String')
@secure()
param azureSQLConnectionString string

// Variables
var resourceNames = {
  appServicePlanName: 'asp-${applicationName}-${location}-001'
  appName : 'aps-${applicationName}-${location}'
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: resourceNames.appServicePlanName
  location: location
  tags: tags
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: true
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
  }
}

resource app 'Microsoft.Web/sites@2022-03-01' = {
  name: resourceNames.appName
  location: location
  tags: tags
  kind: 'app,linux'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${resourceNames.appName}.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${resourceNames.appName}.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: appServicePlan.id
    reserved: true
    isXenon: false
    hyperV: false
    vnetRouteAllEnabled: false
    vnetImagePullEnabled: false
    vnetContentShareEnabled: false
    siteConfig: {
      numberOfWorkers: 1
      linuxFxVersion: 'DOTNETCORE|8.0'
      acrUseManagedIdentityCreds: false
      alwaysOn: false
      http20Enabled: false
      functionAppScaleLimit: 0
      minimumElasticInstanceCount: 0
    }
    scmSiteAlsoStopped: false
    clientAffinityEnabled: false
    clientCertEnabled: false
    clientCertMode: 'Required'
    hostNamesDisabled: false
    customDomainVerificationId: '863D812A4F8321ABD7EE56AC999CCEA38C9856F34D6BB6D836065FB757627DF1'
    containerSize: 0
    dailyMemoryTimeQuota: 0
    httpsOnly: true
    redundancyMode: 'None'
    storageAccountRequired: false
    keyVaultReferenceIdentity: 'SystemAssigned'
  }
}

resource appAppSettings 'Microsoft.Web/sites/config@2022-03-01' = if(!empty(appiConnectionString)) {
  name: 'web'
  parent: app
  properties: {
    connectionStrings: [
      {
        name: 'SQLConnectionString'
        connectionString: azureSQLConnectionString
      }
      {
        name: 'RedisConnectionString'
        connectionString: '${redisHostName}:10000, password=${redisPassword}, ssl=true'    
      }
    ]
  }
}

output appHostName string = app.properties.defaultHostName
