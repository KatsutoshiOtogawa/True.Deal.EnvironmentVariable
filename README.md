
## True.Deal.EnvironmentVariable

windowsの環境変数にはバグがあります。
そのバグをMSがなんとかする気が無くて、dotnetのissueでもpwshのissueでも
ずっと蹴られていて、解決する事が一生なさそうなのでここにバグを回避するための
windowsの環境変数操作用の処理を置きます。

There is a bug in windows and dotnet that can break environment variables.
This library is designed to avoid those bugs and manipulate environment variables.

https://github.com/dotnet/runtime/issues/1442

## Supported platforms

It is compatible with most of the windows environments in use today.

- .NET Framework 3.5+
- .NET Standard 2.0+
- dotnet6.0+

Net Framework 4.5 or lower version should work, although I have not tested it because I do not have a verification environment.

## how to use

整備中

```csharp

// get User EnvironmentVariable
// get User EnvironemntVariable 
// True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentValueKind("fooEnvironmentVariable", EnvironmentVariableTarget.User);

// set User EnvironmentVariable with RegistryValueKind

```

## powershellで使いたい

まだ、0系列ですが、 下記のPSGalleryにモジュールがあるのでそれを使ってください。

[PSGallery](https://www.powershellgallery.com/packages/True.Deal.EnvironmentVariable/)
