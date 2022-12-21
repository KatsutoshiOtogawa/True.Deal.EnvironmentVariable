
## 

windowsの環境変数にはバグがあります。
そのバグをMSがなんとかする気が無くて、dotnetのissueでもpwshのissueでも
ずっと蹴られていて、解決する事が一生なさそうなのでここにバグを回避するための
windowsの環境変数操作用の処理を置きます。

## support environment

dotnet 6, dotnet 7ランタイム環境のみ。

テストの煩雑さと.net frameworkの貧弱さからかなり手間がかかることが予想されます。
.net frameworkに対応する気は今のところありません。

## development 

###

```powershell
winget install Microsoft.DotNet.SDK.6
winget install Microsoft.DotNet.SDK.7
```

## release build

bin\Release\に出力されます。

```bash
dotnet pack -c Release
```

## powershellでの使い方

じきにPSGalleryにパッケージもアップロードするつもりです。

nuget orgのサイトからダウンロードして
nupkgファイルの拡張子を.zipに変更、zipファイルを展開してください。

dotnet6.0, 7.0 ベースのpwsh(version 6.3, 7.0)で下記のコマンドを打ちます。

```powershell
# pwshのバージョンと対応している.netのバージョンを使う。
Add-type -Path "true.deal.environmentvariable.0.1.1-alpha\lib\net6.0\True.Deal.EnvironmentVariable.dll"


[True.Deal.EnvironmentVariable.Environment]::WinGetEnvironmentVariables()
```