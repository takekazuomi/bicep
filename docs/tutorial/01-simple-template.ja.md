# Working with a basic bicep file

In this tutorial we'll start from a blank file and build up to a file with the basic bicep primitives.

If you haven't already, follow [these instructions](../installing.md) to install the bicep CLI and VS Code extension.

このチュートリアルでは、空白のファイルから始めて、基本的な bicep プリミティブを持つファイルまで構築します。

まだインストールしていない場合は、[これらの指示](./installing.md)に従って、bicep CLI と VS Code 拡張モジュールをインストールしてください。

## Compile an empty bicep file

Let's start by creating a blank file `main.bicep` and compiling it by running:

まずは空白のファイル `main.bicep` を作成し、実行してコンパイルしてみましょう。

```bash
bicep build main.bicep
```

You should get an output json file of the same name in your current directory -- in this case `main.json`. It should be a skeleton ARM JSON template:

現在のディレクトリに同じ名前の出力jsonファイルを取得する必要があります -- この場合は `main.json` です。これは、スケルトンARM JSONテンプレートでなければなりません。

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {},
  "functions": [],
  "variables": {},
  "resources": [],
  "outputs": {}
}
```

## Add a resource

Next we will add our first `resource` to our bicep file -- a basic storage account.

次に、最初の `resource` を bicep ファイルに追加します。

```
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: 'uniquestorage001' // must be globally unique
    location: 'eastus'
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
}
```

The resource declaration has four components:

リソース宣言には4つのコンポーネントがあります。

* `resource` keyword
* **symbolic name** (`stg`) - this is an identifier for referencing the resource throughout your bicep file. It is *not* what the name of the resource will be when it's deployed.
* **type** (`Microsoft.Storage/storageAccounts@2019-06-01`) - composed of the resource provider (`Microsoft.Storage`), resource type (`storageAccounts`), and apiVersion (`2019-06-01`). These properties should be familiar if you've ever deployed ARM Templates before. You can find more types and apiVersions for various Azure resources [here](https://docs.microsoft.com/en-us/rest/api/resources/).
* **properties** (everything inside `= {...}`) - these are the specific properties you would like to specify for the given resource type. These are *exactly* the same properties available to you in an ARM Template.

* `resource` キーワード
* **symbolic name** (`stg`) - これは bicep ファイル全体でリソースを参照するための識別子です。リソースがデプロイされたときに、リソースの名前が何になるかは *not* です。
* **type** (`Microsoft.Storage/storageAccounts@2019-06-01`) - リソースプロバイダ (`Microsoft.Storage`)、リソースタイプ (`storageAccounts`)、apiVersion (`2019-06-01`) から構成されます。これらのプロパティは、以前にARMテンプレートをデプロイしたことがある人には馴染みがあるはずです。様々なAzureリソースのタイプとapiVersionはこちらで確認できます[here](https://docs.microsoft.com/en-us/rest/api/resources/)
* **properties** (`= {...}`内のすべてのプロパティ) - これらは、指定されたリソースタイプに指定したい特定のプロパティです。これらは、ARM テンプレートで利用可能なプロパティとまったく同じです。

When we compile the template with `bicep build main.bicep`, we see the following JSON:

テンプレートを `bicep build main.bicep` でコンパイルすると、以下のようなJSONが表示されます。

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {},
  "functions": [],
  "variables": {},
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "uniquestorage001",
      "location": "eastus",
      "kind": "Storage",
      "sku": {
        "name": "Standard_LRS"
      }
    }
  ],
  "outputs": {}
}
```

At this point, I can deploy it like any other ARM template using the standard command line tools (`az deployment group create ...` or `New-AzResourceGroupDeployment ...`).

この時点で、標準のコマンドラインツール（`az deployment group create ...` または `New-AzResourceGroupDeployment ...`）を使用して、他のARMテンプレートと同じようにデプロイすることができます。

## Add parameters

In most cases, I want to expose the resource name and the resource location via parameters, so I can add the following:

ほとんどの場合、リソース名とリソースの場所をパラメータで公開したいので、以下のように追加します。

```
param location string = 'eastus'
param name string = 'uniquestorage001' // must be globally unique

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: name
    location: location
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
}
```

Notice the `parameters` can be referenced directly via their name in bicep, compared to requiring `[parameters('location')]` in ARM template JSON.

ARMのテンプレートJSONでは`[parameters('location')]`が必要ですが、bicepでは`parameters`を直接参照することができます。

The end of the parameter declaration (`= 'eastus'`) is only the *default* value. It can be optionally overridden at deployment time.

パラメータ宣言の最後 (`= 'eastus'`) は *default* の値だけです。デプロイ時にオプションで上書きすることができます。

Let's compile with `bicep build main.bicep` and look at the output:

bicep build main.bicep`でコンパイルして出力を見てみましょう。

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "location": {
      "type": "string",
      "defaultValue": "eastus"
    },
    "name": {
      "type": "string",
      "defaultValue": "uniquestorage001"
    }
  },
  "functions": [],
  "variables": {},
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "[parameters('name')]",
      "location": "[parameters('location')]",
      "kind": "Storage",
      "sku": {
        "name": "Standard_LRS"
      }
    }
  ],
  "outputs": {}
}
```

## Add variables and outputs

I can also add `variables` for storing values or complex expressions, and emit `outputs` to be passed to a script or another template:

また、値や複雑な式を格納するための `変数` を追加したり、スクリプトや他のテンプレートに渡すための `出力` を出力したりすることもできます。

```
param location string = 'eastus'
param name string = 'uniquestorage001' // must be globally unique

var storageSku = 'Standard_LRS' // declare variable and assign value

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: name
    location: location
    kind: 'Storage'
    sku: {
        name: storageSku // reference variable
    }
}

output storageId string = stg.id // output resourceId of storage account
```

Notice I can easily reference the resource `Id` from the symbolic name of the storage account (`stg.id`) which we will translate to the `resourceId(...)` function in the compiled template. When compiled, you should see the following ARM Template JSON:

ストレージアカウントのシンボリック名 (`stg.id`) からリソース `Id` を簡単に参照できることに注意してください。コンパイルすると、以下のようなARMテンプレートJSONが表示されるはずです。

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "location": {
      "type": "string",
      "defaultValue": "eastus"
    },
    "name": {
      "type": "string",
      "defaultValue": "uniquestorage001"
    }
  },
  "functions": [],
  "variables": {
    "storageSku": "Standard_LRS"
  },
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "[parameters('name')]",
      "location": "[parameters('location')]",
      "kind": "Storage",
      "sku": {
        "name": "[variables('storageSku')]"
      }
    }
  ],
  "outputs": {
    "storageId": {
      "type": "string",
      "value": "[resourceId('Microsoft.Storage/storageAccounts', parameters('name'))]"
    }
  }
}
```

## Next steps

In the next tutorial, we will walk through compiling and deploying a bicep file:

次のチュートリアルでは、bicep fileのコンパイルとデプロイについて説明します。

[2 - Deploying a bicep file](./02-deploying-a-bicep-file.md)
