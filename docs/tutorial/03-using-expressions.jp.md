# Using Bicep expressions

In the first tutorial, we declared a basic storage account resource and augmented that declaration with references to declared parameters and variables. These references are forms of **expressions**.

Even though the bicep language is still in its infancy, you can already write some powerful expressions to take advantage of in your bicep files.

最初のチュートリアルでは、基本的なストレージ・アカウント・リソースを宣言し、宣言されたパラメータと変数への参照でその宣言を拡張しました。これらの参照は **expressions** の形式です。

bicep言語はまだ黎明期にあるにもかかわらず、すでにいくつかの強力な式を書くことができ、bicepファイルの中でそれを利用することができます。

## Using a bicep function

Any valid [ARM Template function](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions) is also a valid bicep function. Resource functions do not yet understand new concepts like the symbolic resource name, so they still require a resourceId.

The following are all valid function calls in bicep:

有効な[ARM Template function](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions)も有効なbicep 関数です。リソース関数は、シンボリックリソース名のような新しい概念をまだ理解していないため、resourceIdが必要です。

以下は、bicepでのすべての有効な関数呼び出しです。

```
param serverFarmId string = resourceId('Microsoft.Web/sites', 'myWebsite')

var location = resourceGroup().location

output makeCapital string = toUpper('all lowercase')
```

こんな風になる。

```
% az deployment group create -g $g -n test01 -f ./exp01.json --query properties.outputs
{- Finished ..
  "makeCapital": {
    "type": "String",
    "value": "ALL LOWERCASE"
  }
}
```

In our `main.bicep` file, instead of forcing users to guess a unique storage account name, let's get rid of our `name` parameter and use the `uniqueString()` and `resourceGroup()` functions to calculate a unique name. We'll also use the `resourceGroup().location` property instead of hardcoding a default location.

`main.bicep` ファイルでは、ユーザに一意なストレージアカウント名を推測させる代わりに、`name` パラメータを削除して `uniqueString()` と `resourceGroup()` 関数を使用して一意な名前を計算してみましょう。また、デフォルトの場所をハードコーディングする代わりに `resourceGroup().location` プロパティを使用します。

```
param location string = resourceGroup().location

var storageSku = 'Standard_LRS' // declare variable and assign value

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: uniqueString(resourceGroup().id) // generates unique name based on resource group ID
    location: location
    kind: 'Storage'
    sku: {
        name: storageSku // assign variable
    }
}

output storageId string = stg.id
```

## Using string interpolation

The `concat()` function is one of the most commonly used ARM Template functions and can add a lot of verbosity to a template. To simplify this, we now support a [string interpolation](https://en.wikipedia.org/wiki/String_interpolation#) syntax.

For example, I could combine a `namePrefix` parameter with a hardcoded suffix:

関数 `concat()` は、最も一般的に使用されている ARM テンプレート関数の 1 つであり、テンプレートに多くの冗長性を追加することができます。これを単純化するために、[string interpolation](https://en.wikipedia.org/wiki/String_interpolation#)構文をサポートしています。

例えば、`namePrefix` パラメータとハードコードされた接尾辞を組み合わせることができます。

```
param namePrefix string = 'unique'

var storageAccountName = '${namePrefix}storage001'
```

Which is equivalent to the following ARM Template `concat()` function:

これは以下のARMテンプレートの `concat()` 関数と同等です。

```json
{
  "variables": {
    "storageAccountName": "[concat(parameters('namePrefix'), 'storage001')]"
  }
}
```

こうなった、ドキュメントが古いのかな？

```json
  "variables": {
    "storageAccountName": "[format('{0}storage001', parameters('namePrefix'))]"
  },
```

Let's use a `namePrefix` parameter and concatenate that with our `uniqueString()` using string interpolation. We'll also use the variable `storageAccountName` to store this expression, so that our resource declaration is a bit cleaner:

パラメータに `namePrefix` を指定し、それを文字列補間を用いて `uniqueString()` と連結してみましょう。また、この式を格納するために変数 `storageAccountName` を使用します。

```
param location string = resourceGroup().location
param namePrefix string = 'stg'

var storageSku = 'Standard_LRS' // declare variable and assign value
var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    kind: 'Storage'
    sku: {
        name: storageSku
    }
}

output storageId string = stg.id
```

If you compile with `bicep build`, you will notice we are compiling this into the `format()` function, not the `concat()` function. Functionally, the end result is the same, but the `format()` function is a bit more stable in the IL.

`bicep build`でコンパイルすると、これを `concat()` 関数ではなく `format()` 関数にコンパイルしていることに気づくだろう。機能的には最終的な結果は同じですが、`format()` 関数の方がILでは少し安定しています。

こんな風になってた。

```json
  "variables": {
    "storageSku": "Standard_LRS",
    "storageAccountName": "[format('{0}{1}', parameters('namePrefix'), uniqueString(resourceGroup().id))]"
  },
```

## Using the ternary operator

You can conditionally provide a value for a variable, resource, or output using the [ternary operator](https://en.wikipedia.org/wiki/%3F:), which is the equivalent of the `if()` function in ARM Templates. Instead of using a variable for our storage sku, let's conditionally choose a redundancy setting for our storage account by adding a new parameter `globalRedundancy` and combining it with the ternary operator. As part of that, we can get rid of our `storageSku` variable.

ARMテンプレートの`if()`関数に相当する[ternary operator](https://en.wikipedia.org/wiki/%3F:)を使用して、変数、リソース、または出力に条件付きで値を与えることができます。ストレージアカウントに変数を使用する代わりに、新しいパラメータ `globalRedundancy` を追加して三項演算子と組み合わせることで、条件付きでストレージアカウントの冗長性設定を選択してみましょう。その一環として、変数 `storageSku` を取り除くことができます。


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
```

## Next steps

In the next tutorial, we will learn how use the resource's **symbolic name** for simple property references and calculating dependencies:

次のチュートリアルでは、単純なプロパティ参照や依存関係の計算にリソースの**symbolic name**を使用する方法を学びます。

[4 - Using the symbolic resource name](./04-using-symbolic-resource-name.md)
