
## 

windowsの環境変数にはバグがあります。
そのバグをMSがなんとかする気が無くて、dotnetのissueでもpwshのissueでも
ずっと蹴られていて、解決する事が一生なさそうなのでここにバグを回避するための
windowsの環境変数操作用の処理を置きます。

## support environment

dotnet 6, dotnet 7ランタイム環境のみ。

テストの煩雑さと.net frameworkの貧弱さからかなり手間がかかることが予想されます。
.net frameworkに対応する気は今のところありません。

https://www.neko3cs.net/entry/2020/03/14/095857
System.Management.Automation
## development 

### dotnet

最終的なビルドはコマンドラインから行うので下の者が必要。

```powershell
winget install Microsoft.DotNet.SDK.6
winget install Microsoft.DotNet.SDK.7
```

### powershell

powershell開発用のモジュールをインストールする。
dotnet standard

dotnet new -i Microsoft.PowerShell.Standard.Module.Template

### visual studio

powershellの開発用モジュール作成に必要。

[](https://marketplace.visualstudio.com/items?itemName=AdamRDriscoll.PowerShellToolsVS2022)

## release build

bin\Release\に出力されます。

```bash
cd Resgen/
# dotnet net6.0ように出力
# 吐くソースコードはdotnet run --framework net6.0でもnet7.0でも変わらない。
# 指定しないと動かないので指定している。
dotnet run --framework net6.0
cd ../
dotnet pack -c Release

Test-Modules
```

## powershellでの使い方

じきにPSGalleryにパッケージもアップロードするつもりです。

nuget orgのサイトからダウンロードして
nupkgファイルの拡張子を.zipに変更、zipファイルを展開してください。

dotnet6.0, 7.0 ベースのpwsh(version 7.2, 7.3)で下記のコマンドを打ちます。

```powershell
# pwshのバージョンと対応している.netのバージョンを使う。
Add-type -Path "true.deal.environmentvariable.0.1.1-alpha\lib\net6.0\True.Deal.EnvironmentVariable.dll"


[True.Deal.EnvironmentVariable.Environment]::WinGetEnvironmentVariables()


Import-Module
```

$env:PSModulePath = $TestModulePath+$TestModulePathSeparator+$($env:PSModulePath)

現在のセッションにpublish配下のプロジェクトを入れる。

$publishPath = Resolve-path publish
$Env:PSModulePath =  "$($publishPath.Path);$($Env:PSModulePath)"

Publish-Module -Name "True.Deal.EnvironmentVariable" -NugetApiKey $env:NUGET_API 
-LicenseUri `

