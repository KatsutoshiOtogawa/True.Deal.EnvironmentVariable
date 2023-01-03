
## 

windowsの環境変数にはバグがあります。
そのバグをMSがなんとかする気が無くて、dotnetのissueでもpwshのissueでも
ずっと蹴られていて、解決する事が一生なさそうなのでここにバグを回避するための
windowsの環境変数操作用の処理を置きます。

## support environment

dotnet 6, dotnet 7, netstandard2.0環境のみ。


https://www.neko3cs.net/entry/2020/03/14/095857
System.Management.Automation
## development 

### dotnet

最終的なビルドはコマンドラインから行うので下の者が必要。

```powershell
winget install Microsoft.DotNet.SDK.6
winget install Microsoft.DotNet.SDK.7
```

net452以前のビルドには下記の上でvisual studio 2017以前が必要になる。
ビルドしなくてもエラーを消すためにインストールする必要がある。
Turn Windows Feature on off -> .Net framework 3.5 check.
## test

### netcore系
dotnet test
### .net framework系
visual studioから実行するしかない。
```powershell
 & "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
 ```


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

dotnet nuget push .\True.Deal.EnvironmentVariable\bin\Release\True.Deal.EnvironmentVariable.0.5.0.nupkg -k $Env:NUGET_API

Test-Modules


 & "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" /p:TargetFrameworkVersion=v4.5
 & 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' /p:TargetFrameworkVersion=v4.8 /t:True_Deal_EnvironmentVariable
```

$env:PSModulePath = $TestModulePath+$TestModulePathSeparator+$($env:PSModulePath)

現在のセッションにpublish配下のプロジェクトを入れる。

$publishPath = Resolve-path publish
$Env:PSModulePath =  "$($publishPath.Path);$($Env:PSModulePath)"

Publish-Module -Name "True.Deal.EnvironmentVariable" -NugetApiKey $env:NUGET_API 
-LicenseUri `

## powershellで使う場合

まだベータかつGet-WinEnvironmentVariableしか使えませんが、下記のサイトにレポジトリがあります。

[Powershell.Commands.True.Deal.EnvironmentVariable](https://github.com/KatsutoshiOtogawa/PowerShell.Commands.True.Deal.EnvironmentVariable)



