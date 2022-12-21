
## development 

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
