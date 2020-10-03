# Using the symbolic resource name

Let's take another look at our storage account. As we discussed earlier, the resource declaration includes a **symbolic name**, which is separate from the name of the resource that will be created. The symbolic name is simply a reference to the resource within bicep, and it is a very useful pointer for retrieving properties of a resource and in turn keeping your syntax readable and easy to manage.

ストレージアカウントをもう一度見てみましょう。先ほど説明したように、リソースの宣言には **symbolic name** が含まれており、これは作成されるリソースの名前とは別のものです。シンボリック名は単にbicep内のリソースへの参照であり、リソースのプロパティを取得するための非常に便利なポインタであり、ひいては構文を読みやすく管理しやすくしてくれます。

## Retrieving the resource ID

Instead of using the `resourceId()` function, you can simply use the identifier and retrieve the relevant property. We've already done this with our output `stg.id`. In ARM Template JSON this will compile to `[resourceId('Microsoft.Storage/storageAccounts', parameters('name'))]`.

関数 `resourceId()` を使う代わりに、単に識別子を使って関連するプロパティを取得することができます。これはすでに出力された `stg.id` で行っています。ARMテンプレートJSONでは、これは `[resourceId('Microsoft.Storage/storageAccounts', parameters('name'))]` にコンパイルされます。

## Retrieving other "compile time" properties

Bicep special cases a few properties for which it will use the "compile-time" value. These include:

Bicepは、"compile-time" の値を使用するいくつかのプロパティを特別に扱います。これらには以下が含まれます。

* `id`
* `name`
* `apiVersion`
* `type`

To test this, we can add another `output` with the storage account name, which is useful to know since that `uniqueString` will be generated when the template is deployed.

これをテストするために、ストレージアカウント名を使用して別の `output`を追加します。これは、テンプレートのデプロイ時に`uniqueString`が生成されるため、知っておくと役立ちます。

```
param location string = resourceGroup().location
param namePrefix string = 'stg'

param globalRedundancy bool = true // defaults to true, but can be overridden

var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: globalRedundancy ? 'Standard_GRS' : 'Standard_LRS' // if true --> GRS, else --> LRS
    }
}

output storageId string = stg.id
output computedStorageName string = stg.name
```

You may find you need properties other properties like `apiVersion` for functions like [list*](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions-resource#list).

[list*](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions-resource#list)のような関数には、`apiVersion`のようなプロパティが必要になるかもしれません。

## Retrieving "run-time" properties

In ARM Templates, I use the `reference()` function to retrieve run-time properties that only exist after the resource has been created. In a storage account, the `primaryEndpoints` property is one of those. Let's add another output to retrieve this run-time property:

ARMテンプレートでは、リソースが作成された後にのみ存在するランタイムプロパティを取得するために `reference()` 関数を使用します。ストレージアカウントでは、`primaryEndpoints` プロパティがその一つです。このランタイムプロパティを取得するために、別の出力を追加してみましょう。

```
param location string = resourceGroup().location
param namePrefix string = 'stg'

param globalRedundancy bool = true // defaults to true, but can be overridden

var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: globalRedundancy ? 'Standard_GRS' : 'Standard_LRS' // if true --> GRS, else --> LRS
    }
}

output storageId string = stg.id
output computedStorageName string = stg.name
output blobEndpoint string = stg.properties.primaryEndpoints.blob // replacement for reference(...).*
```

When we compile, we see the ARM template is correctly using the `reference()` function:

コンパイルすると、ARMテンプレートが `reference()` 関数を正しく使用していることがわかります。

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "location": {
      "type": "string",
      "defaultValue": "[resourceGroup().location]"
    },
    "namePrefix": {
      "type": "string",
      "defaultValue": "stg"
    },
    "globalRedundancy": {
      "type": "bool",
      "defaultValue": true
    }
  },
  "functions": [],
  "variables": {
    "storageAccountName": "[format('{0}{1}', parameters('namePrefix'), uniqueString(resourceGroup().id))]"
  },
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "[variables('storageAccountName')]",
      "location": "[parameters('location')]",
      "kind": "Storage",
      "sku": {
        "name": "[if(parameters('globalRedundancy'), 'Standard_GRS', 'Standard_LRS')]"
      }
    }
  ],
  "outputs": {
    "storageId": {
      "type": "string",
      "value": "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
    },
    "computedStorageName": {
      "type": "string",
      "value": "[variables('storageAccountName')]"
    },
    "blobEndpoint": {
      "type": "string",
      "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))).primaryEndpoints.blob]"
    }
  }
}
```

## Automatically calculate dependsOn

In ARM Templates today, you must manually specify resource dependencies with the `dependsOn` property. If we want to create a blob container in our storage account, we need to make sure the storage account gets created first. With bicep, if you reference any property of the resource via the symbolic name, **we will automatically add the dependsOn property**.

今日のARMテンプレートでは、`dependsOn`プロパティでリソースの依存関係を手動で指定する必要があります。ストレージアカウントにブロブコンテナを作成したい場合は、ストレージアカウントが最初に作成されることを確認する必要があります。bicepでは、シンボリック名を介してリソースの任意のプロパティを参照すると、**we will automatically add the dependsOn property** が追加されます。

Let's add a new resource for our blob container to our storage account, and reference the `name` property via symbolic name (`stg.name`):

blobコンテナ用の新しいリソースをストレージアカウントに追加し、シンボリック名 (`stg.name`) で `name` プロパティを参照してみましょう。

```
param location string = resourceGroup().location
param namePrefix string = 'stg'

param globalRedundancy bool = true // defaults to true, but can be overridden

var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'Storage'
  sku: {
    name: globalRedundancy ? 'Standard_GRS' : 'Standard_LRS' // if true --> GRS, else --> LRS
  }
}

resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${stg.name}/default/logs'
  // dependsOn will be added when the template is compiled
}

output storageId string = stg.id // replacement for resourceId(...)
output computedStorageName string = stg.name
output primaryEndpoint string = stg.properties.primaryEndpoints.blob // replacement for reference(...).*
```

We can see in our compiled ARM Template that the `dependsOn` is correctly added to the child resource.

コンパイルしたARMテンプレートでは、`dependsOn` が子リソースに正しく追加されていることがわかります。


```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "location": {
      "type": "string",
      "defaultValue": "[resourceGroup().location]"
    },
    "namePrefix": {
      "type": "string",
      "defaultValue": "stg"
    },
    "globalRedundancy": {
      "type": "bool",
      "defaultValue": true
    }
  },
  "functions": [],
  "variables": {
    "storageAccountName": "[format('{0}{1}', parameters('namePrefix'), uniqueString(resourceGroup().id))]"
  },
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "[variables('storageAccountName')]",
      "location": "[parameters('location')]",
      "kind": "Storage",
      "sku": {
        "name": "[if(parameters('globalRedundancy'), 'Standard_GRS', 'Standard_LRS')]"
      }
    },
    {
      "type": "Microsoft.Storage/storageAccounts/blobServices/containers",
      "apiVersion": "2019-06-01",
      "name": "[format('{0}/default/logs', variables('storageAccountName'))]",
      "dependsOn": [
        "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
      ]
    }
  ],
  "outputs": {
    "storageId": {
      "type": "string",
      "value": "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
    },
    "computedStorageName": {
      "type": "string",
      "value": "[variables('storageAccountName')]"
    },
    "primaryEndpoint": {
      "type": "string",
      "value": "[reference(resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))).primaryEndpoints.blob]"
    }
  }
}
```

## Next steps

In the final tutorial, we will learn how to convert an arbitrary ARM template into a bicep file:

最後のチュートリアルでは、任意のARMテンプレートをbicepファイルに変換する方法を学びます。

[5 - Convert any ARM template into a Bicep file](./05-convert-arm-template.md)
