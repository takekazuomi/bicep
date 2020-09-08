![Build](https://github.com/Azure/bicep/workflows/Build/badge.svg) [![codecov](https://codecov.io/gh/Azure/bicep/branch/master/graph/badge.svg)](https://codecov.io/gh/Azure/bicep)

# Project Bicep - an ARM DSL

***Note:** Bicep is currently an **experimental language** and we expect to ship breaking changes in future releases. It is not yet recommended for production usage. Please take a look at the [known limitations](#known-limitations) prior to opening any issues.*

## What is Bicep?

Bicep is a Domain Specific Language (DSL) for deploying Azure resources declaratively. It aims to drastically simplify the authoring experience with a cleaner syntax and better support for modularity and code re-use. Bicep is a **transparent abstraction** over ARM and ARM templates, which means anything that can be done in an ARM Template can be done in bicep (outside of temporary [known limitations](#known-limitations)). All resource `types`, `apiVersions`, and `properties` that are valid in an ARM template are equally valid in Bicep on day one.

Bicep compiles down to standard ARM Template JSON files, which means the ARM JSON is effectively being treated as an Intermediate Language (IL).

Bicepは、Azureリソースを宣言的にデプロイするためのドメイン固有言語(DSL)です。よりクリーンな構文、モジュール性とコードの再利用のためのより良いサポートにより、オーサリングの経験を大幅に簡素化することを目的としています。Bicepは、ARMとARMテンプレート上の**透過的な抽象化**であり、ARMテンプレートでできることはすべてBicepでできることを意味します（一時的な[known limitations](#known-limitations)を除いて）。ARMテンプレートで有効なすべてのリソース `types`、`apiVersions`、および `property` は、初日にBicepでも同じように有効です。

Bicepは標準のARMテンプレートJSONファイルにコンパイルされますが、これはARM JSONが実質的に中間言語（IL）として扱われることを意味します。

## Goals

1. Build the best possible language for describing, validating, and deploying infrastructure to Azure.
1. The language should provide a *transparent abstraction* for the underlying platform. There must be no "onboarding step" to enable it to a new resource `type` and/or `apiVersion` in Bicep.
1. Code should be easy to understand at a glance and straightforward to learn, regardless of your experience with other programming languages.
1. Users should be given a lot of freedom to modularize and reuse their code. Reusing code should not require any 'copy/paste'.
1. Tooling should provide a high level of resource discoverability and validation, and should be developed alongside the compiler rather than added at the end.
1. Users should have a high level of confidence that their code is 'syntactically valid' before deploying.

1. インフラストラクチャをAzureに記述、検証、デプロイするための最良の言語を構築する。
1. 言語は、基礎となるプラットフォームのための*透過的な抽象化*を提供する必要があります。Bicepの新しいリソース`type`および/または`apiVersion`に有効にするための「オンボーディングステップ」があってはなりません。
1. コードは、他のプログラミング言語の経験に関わらず、一目で理解しやすく、学習しやすいものでなければならない。
1. ユーザーは、自分のコードをモジュール化して再利用する自由度が高いこと。コードの再利用には「コピー＆ペースト」を必要としないこと。
1. ツールは、高レベルのリソース発見性と検証性を提供し、最後に追加するのではなく、コンパイラと一緒に開発されるべきです。
1. ユーザーは、デプロイする前に、自分のコードが「構文的に有効」であることを高いレベルで確信しなければなりません。

### Non-goals

1. Build a general purpose language to meet any need. This will not replace general purpose languages and you may still need to do pre or post bicep tasks in a script or high-level programming language.
1. Provide a first-class provider model for non-Azure related tasks. While we will likely introduce an extensibility model at some point, any extension points are intended to be focused on Azure infra or application deployment related tasks.

1. あらゆるニーズに対応できる汎用言語を構築する。これは汎用言語に取って代わるものではなく、スクリプトや高レベルのプログラミング言語でbicepの前後のタスクを行う必要があるかもしれません。
1. 非Azure関連のタスクのためのファーストクラスのプロバイダモデルを提供する。拡張性モデルはどこかの時点で導入されると思われますが、拡張ポイントはAzure infraやアプリケーションのデプロイに関連するタスクに焦点を当てることを意図しています。

## Get started with Bicep

To get going with bicep, start by [installing the tooling](./docs/installing.md).

Once the tooling is installed, you can start the [bicep tutorial](./docs/tutorial/01-simple-template.md), which walks you through the structure and capabilities of bicep, deploying bicep files, and converting an ARM template into the equivalent bicep file.

Alternatively, you can try the [Bicep Playground](https://aka.ms/bicepdemo).

bicepを始めるには、まず [installing the tooling](./docs/installing.md) から始めます。

ツールがインストールされたら、[bicep tutorial](./docs/tutorial/01-simple-template.md)を開始することができます。このチュートリアルでは、bicepの構造と機能、bicepファイルのデプロイ、ARMテンプレートを同等のbicepファイルに変換する方法を説明しています。

あるいは、[Bicep Playground](https://aka.ms/bicepdemo)を試すこともできます。

## How does Bicep work?

First, author your Bicep code using the Bicep language service as part of the [Bicep VS Code extension](./docs/installing.md#bicep-vs-code-extension), then compile that code into an ARM template using the [Bicep CLI](./docs/installing.md#bicep-cli):

まず、[Bicep VS Code extension](./docs/installing.md#bicep-vs-code-extension)の一部としてBicep言語サービスを使用してBicepコードをオーサリングし、そのコードを[Bicep CLI](./docs/installing.md#bicep-cli)を使用してARMテンプレートにコンパイルします。

```bash
bicep build ./main.bicep
```

You should see the compiled template `main.json` get created in the same directory as `main.bicep`. Now that you have the ARM Template, you can use all existing ARM Template tooling such as [what-if](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-deploy-what-if?tabs=azure-powershell), the [ARM Tools Extension](https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools), and the deployment CLI commands.

You can now deploy this template via any method you would like such as:

コンパイルされたテンプレート `main.json` が `main.bicep` と同じディレクトリに作成されているのがわかるはずです。ARMテンプレートができたので、[what-if](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-deploy-what-if?tabs=azure-powershell)、[ARM Tools Extension](https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools)、デプロイメントCLIコマンドなど、既存のすべてのARMテンプレートツールを使用することができます。

これで、以下のような任意の方法でこのテンプレートをデプロイすることができます。

```bash
az deployment group create -f ./main.json -g my-rg
```

## How is life better with Bicep?

* Simpler syntax when compared to equivalent JSON
  * No special `[...]` expressions syntax required
    * Directly call parameters or variables in expressions without a function (no more need for `parameters('myParam')`)
  * No quotes on property names (e.g. `"location"`)
  * Simple string interpolation: `'${namePrefix}-vm'` instead of `concat(parameters('namePrefix'), '-vm')`
  * Simpler resource declaration using positional properties to avoid typing common property names like `type` and `apiVersion` explicitly.
  * Direct property access of a resource (e.g. `aks.properties.fqdn` instead of `reference(parameters('aksName')).properties.fqdn`)
* Better copy/paste experience via flexible declaration of types. Different types (e.g. `variables`, `resources`, `outputs`) can be declared anywhere.
  * Previously all parameters had to be declared together in one `"parameters": {}` object, variables had to be declared together in one `"variables": {}` object, etc.
* Automatic dependency management in certain scenarios. Bicep will automatically add `dependsOn` in the compiled ARM Template if the symbolic name is used in another resouce declaration.

* 同等のJSONと比較して、よりシンプルな構文
  * 特別な `[...]` 式の構文は必要ありません。
    * 関数を使わずに、式の中のパラメータや変数を直接呼び出すことができます（`parameters('myParam')`は不要です）。
  * プロパティ名に引用符を使用しない（例："location"`）。
  * 単純な文字列補間: `concat(parameters('namePrefix'), '-vm')` の代わりに `'${namePrefix}-vm'` を使用します。
  * `type`や`apiVersion`のような一般的なプロパティ名を明示的にタイプする必要がないように、位置プロパティを使用してリソースの宣言をよりシンプルにしました。
  * リソースの直接プロパティアクセス（例：`reference(parameters('aksName')).properties.fqdn`の代わりに `aks.properties.fqdn`）。
* 柔軟な型の宣言により、コピー＆ペーストの操作性が向上しました。異なる型(例: `変数`, `リソース`, `出力`)をどこでも宣言できるようになりました。
  * 以前は、すべてのパラメータを 1 つの `"parameters". {}` オブジェクトにまとめて宣言する必要があり、変数は 1 つの `"variables". オブジェクトにまとめて宣言しなければなりませんでした。
* 特定のシナリオでの自動依存関係管理。シンボリック名が他のリソース宣言で使用されている場合、BicepはコンパイルされたARMテンプレートに自動的に `dependsOn` を追加します。

## Known limitations

* No support for the `copy` or `condition` property [[#185](https://github.com/Azure/bicep/issues/185), [#186](https://github.com/Azure/bicep/issues/186)]
* No explicit support for deployments across scopes (though this can be done by using the `Microsoft.Resources/deployments` resource and using the `templateLink` or `template` property to insert the full ARM template) [[#187](https://github.com/Azure/bicep/issues/187)]
  * Bicep assumes you are deploying to a resource group, though the generated template can be deployed to any scope
* Single line object and arrays (i.e. `['a', 'b', 'c']`) are not yet supported
* You still need to deploy the compiled template yourself, though we plan to build native support for bicep into the powershell `Az` deployment cmdlets and `az cli` deployment commands
* No IntelliSense whatsoever [[#269](https://github.com/Azure/bicep/issues/269)]
* Minimal resource schema validation. Other than basic validations like correct resource `type` structure and requiring a `name`, you will not get errors for missing or incorrect properties in a resource declaration
* No support for string interpolation in property names [[#267](https://github.com/Azure/bicep/issues/267)]
  * From what we know, this only affects using managed identities for resources. You can still include a hardcoded managed identity resource ID (i.e. `'/subscriptions/.../resourceGroups/.../providers/Microsoft.ManagedIdentity/...': {}`)
* Bicep is currently not covered by [Azure support plans](https://azure.microsoft.com/en-us/support/plans/) as it is still in early development stages. Expect Bicep to be covered by all support plans starting on the 0.3 version.

* `copy` や `condition` プロパティはサポートされていません [[#185](https://github.com/Azure/bicep/issues/185), [#186](https://github.com/Azure/bicep/issues/186)
* スコープ間のデプロイメントを明示的にサポートしていません（`Microsoft.Resources/deployments`リソースを使用し、`templateLink`または`template`プロパティを使用して完全なARMテンプレートを挿入することで可能ですが） [[#187](https://github.com/Azure/bicep/issues/187)
  * Bicep はリソースグループにデプロイすることを前提としていますが、生成されたテンプレートはどのスコープにもデプロイできます。
* 単一行オブジェクトと配列 (すなわち `['a', 'b', 'c']`) はまだサポートされていません。
* bicep のネイティブサポートを powershell の `Az` デプロイメントコマンドと `az cli` デプロイメントコマンドに組み込む予定ですが、コンパイルされたテンプレートを自分でデプロイする必要があります。
* インテリセンスは一切なし [[#269](https://github.com/Azure/bicep/issues/269)]
* 最小限のリソーススキーマの検証。リソースの `type` 構造が正しいかどうか、`name` が必要かどうかといった基本的な検証以外は、リソース宣言の中にあるプロパティが欠けていたり間違っていたりしてもエラーは発生しません。
* プロパティ名の文字列補間をサポートしていません[[#267](https://github.com/Azure/bicep/issues/267)]。
  * 私たちの知る限りでは、これはリソースにマネージドアイデンティティを使用する場合にのみ影響します。ハードコードされたマネージド ID リソース ID を含めることができます (例: `'/subscriptions/.../resourceGroups/.../providers/Microsoft.ManagedIdentity/...': {}`)。
* Bicepはまだ開発の初期段階にあるため、現在は[Azure support plans](https://azure.microsoft.com/en-us/support/plans/) ではカバーされていません。Bicep は 0.3 バージョンからすべてのサポートプランでカバーされることを期待しています。

## FAQ

**Why create a new language instead of using an existing one?**
Bicep is more of a revision to the existing ARM template language rather than an entirely new language. While most of the syntax has been changed, the core functionality of ARM templates and the runtime remains the same. You have the same template functions, same resource declarations, etc. Part of the complexity with ARM Templates is due to the "DSL" being embedded inside of JSON. With Bicep, we are revising the syntax of this DSL and moving it into its own `.bicep` file format. Before going down this path, we closely evaluated using an existing high-level programming language, but ultimately determined that bicep would be easier to learn for our target audience. We are open to other implementations of bicep in other languages.

**既存の言語を使用するのではなく、なぜ新しい言語を作成するのでしょうか？**
Bicepは全く新しい言語というよりも、既存のARMテンプレート言語のリビジョンです。構文のほとんどが変更されていますが、ARMテンプレートとランタイムのコア機能は変わりません。同じテンプレート関数、同じリソース宣言などがあります。ARMテンプレートの複雑さの一部は、JSONの内部に「DSL」が埋め込まれていることに起因しています。Bicepでは、このDSLの構文を修正し、独自の`.bicep`ファイル形式に移行しています。この道を進む前に、既存の高レベルプログラミング言語を使って綿密に評価しましたが、最終的にはbicepの方が対象者にとって学びやすいと判断しました。私たちは、他の言語でのbicepの実装を歓迎します。

**Why not focus your energy on Terraform or other third-party IaC offerings?**
Using terraform can be a great choice depending on the requirements of the organization, and if you are happy using terraform there is no reason to switch. At Microsoft, we are actively investing to make sure the terraform on Azure experience is the best it can be.

That being said, there is a huge customer base using ARM templates today because it provides a unique set of capabilities and benefits. We wanted to make the experience for those customers first-class as well, in addition to making it easier to start for Azure focused customers who have not yet transitioned to infra-as-code.

Fundamentally, we believe that configuration languages and tools are always going to be polyglot and different users will prefer different tools for different situations. We want to make sure all of these tools are great on Azure, Bicep is only a part of that effort.

**Terraformや他のサードパーティ製の IaC オファリングにエネルギーを集中させてみませんか？**
組織の要件に応じて、Terraformを使用することは素晴らしい選択であり、Terraformを使用して満足しているのであれば、乗り換える理由はありません。Microsoftでは、Terraform on Azureのエクスペリエンスを最高のものにするために積極的に投資を行っています。

とはいえ、ARMテンプレートを使用している顧客は膨大な数にのぼります。私たちは、まだinfra-as-codeに移行していないAzureに焦点を当てた顧客が簡単に始められるようにするだけでなく、それらの顧客向けのエクスペリエンスも一流にしたいと考えていました。

基本的に、設定言語やツールは常にポリグロットであり、異なるユーザーは異なる状況に応じて異なるツールを好むだろうと考えています。私たちは、これらのツールがすべてAzure上で素晴らしいものであることを確認したいと考えていますが、Bicepはその努力の一部に過ぎません。

**Is this ready for production use? If not, when will it be ready?**
Not yet. We wanted to get the 0.1 release out quickly and get feedback while we still have an opportunity to make breaking changes. By the end of the year, we plan to ship an 0.3 release which will be at parity with what you can accomplish with ARM templates. At that point, we will start recommending production usage.

**これは、生産用として使用する準備ができていますか？準備ができていない場合は、いつ頃になりますか？**
まだです。0.1のリリースを早く出してフィードバックを得たいと考えていました。年末までには、ARMテンプレートで実現できることと同等の0.3リリースを出荷する予定です。その時点で、プロダクションでの使用を推奨し始める予定です。

**What are you looking for feedback on?**
The language syntax and the tooling. Now is the best time to make breaking changes, so syntax feedback is very appreciated.

**何についてフィードバックを求めていますか？**
言語の構文とツールです。今は、画期的な変更を行うのに最適な時期なので、構文のフィードバックは非常に歓迎されています。

**Is this only for Azure?**
Bicep is a DSL focused on deploying end-to-end solutions in Azure. In practice, that usually means working with some non-Azure APIs (i.e. creating kubernetes deployments or users in a database), so we expect to provide some extensibility points. That being said, in the 0.1 release, you can only create Azure resources that are exposed through the ARM API.

**Azure専用ですか？**
BicepはAzureでのエンドツーエンドソリューションのデプロイに特化したDSLです。実際には、それは通常、Azure 以外の API (例えば、kubernetes デプロイメントやデータベース内のユーザーを作成するなど) を使用することを意味します。つまり、0.1リリースでは、ARM APIを通じて公開されているAzureリソースのみを作成することができます。

**What happens to my existing ARM Template investments?**
One of our goals is to make the transition from ARM Templates to Bicep as easy as possible. We plan to ship a "decompiler", which will convert an ARM template into an equivalent Bicep file. We also will support using a standard ARM template as a Bicep module without converting it to bicep.

**既存のARMテンプレート投資はどうなりますか？**
私たちの目標の一つは、ARMテンプレートからBicepへの移行をできるだけ簡単にすることです。ARMテンプレートを同等のBicepファイルに変換する「デコンパイラ」を出荷する予定です。また、標準のARMテンプレートをBicepに変換せずにBicepモジュールとして使用することもサポートする予定です。

## Reference

* [Complete language spec](./docs/spec/bicep.md)
* [@BicepLang](https://twitter.com/BicepLang)
* [ARM Template Reference](https://docs.microsoft.com/azure/templates/)

## Alternatives

Because we are now treating the ARM Template as an IL, we expect and encourage other implementations of IL (ARM Template) generation. We'll keep a running list of alternatives for creating ARM templates that may better fit your use case.

ARM テンプレートを IL として扱うようになったので、IL（ARM テンプレート）生成の他の実装を期待し、奨励します。ARM テンプレートを作成するための代替案のリストは、あなたのユースケースに合ったものにしておきます。

* [Farmer](https://compositionalit.github.io/farmer/) (@isaacabraham) - Generate and deploy ARM Templates on .NET

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
