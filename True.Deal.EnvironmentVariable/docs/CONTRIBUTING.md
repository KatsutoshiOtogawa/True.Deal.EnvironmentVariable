
現在はプルリクは受け付けておりません。
バグについてはissueで受け付けております。

## how to develop

ローカルのwindowsで下記のコマンドを実行してください。

### install dotnet sdk

```powershell
winget install Microsoft.DotNet.SDK.6
winget install Microsoft.DotNet.SDK.7
```

### install .Net framework 3.5

.net framework 3.5を実際使わなくてもインストールしていないと、
visual studio上のビルド、test時にエラーが発生します。

なので必ずインストールしてください。

下記のようにwindowsの機能の有効化、無効化からインストールできます。

Turn Windows Feature on off -> .Net framework 3.5 check.

### nuget コマンド

dotnet nugetは現在、
nugetコマンドの機能すべてを使える分けでない。
特にnuspecファイル作成とか。

```powershell
winget install Microsoft.NuGet
```

### visual studioのインストール

visual studio codeでも開発できますが、
visual studio使った方が楽なので、visual studioをインストールします。

```powershell
winget install Microsoft.VisualStudio.2022.Community
```

## how to build

### build resgen

resxファイルから.resourcesファイル操作用のソースコードを作成します。

```powershell
dotnet run ResGen\ResGen.csproj -f net6.0
```

### build project

dotnet コマンドから.netframework系もdotnet core系もビルドされます。

```powershell
dotnet build .\True.Deal.EnvironmentVariable\True.Deal.EnvironmentVariable.csproj
```

## test

visual studioからtest projectを右クリック -> Test Runから実行してください。

dotnetコマンドからだと
```powershell
dotnet test
dotnet mstest
```

両方やる必要があります。パラメータが微妙に違うし、GUIでやる方が楽です。

## release build

bin\Release\に.nupkgが出力されます。それを使ってください。

```powershell
dotnet pack -c Release .\True.Deal.EnvironmentVariable\True.Deal.EnvironmentVariable.csproj
```

## deploy to nuget.org

```powershell
dotnet nuget push .\True.Deal.EnvironmentVariable\bin\Release\True.Deal.EnvironmentVariable.0.5.0.nupkg -k $Env:NUGET_API
```
