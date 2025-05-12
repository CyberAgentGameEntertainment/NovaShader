# OptionalShaderPass
シェーダーのオプショナルパスを表す列挙型です。

## 値
|値|説明|
|---|---|
|None|オプショナルパスなし|
|DepthOnly|深度値のみを描画するパス。<br/>Depth Prepassが有効な場合に必要となります。|
|DepthNormals|深度値と法線を描画するパス。<br/>Depth Normals Prepassが有効な場合に必要となります。|
|ShadowCaster|シャドウマップを描画するパス。<br/>シャドウキャストを行う場合に必要となります。|

## 使用例
```cs
// シェーダー差し替えの設定
var replaceSettings = new OptimizedShaderReplacer.Settings
{
    // Depth Prepassが実行されるため不透明描画はオプショナルパスにDepthOnlyが必要。
    // また、シャドウキャストも行うためShadowCasterも必要。
    OpaqueRequiredPasses = OptionalShaderPass.DepthOnly | OptionalShaderPass.ShadowCaster,
    CutoutRequiredPasses = OptionalShaderPass.DepthOnly | OptionalShaderPass.ShadowCaster,
    // 半透明描画はDepth Prepassで深度値を描画しないのでDepthOnlyは不要。
    // また、シャドウキャストも行わないので、オプショナルパスは不要
    TransparentRequiredPasses = OptionalShaderPass.None
};
// 設定を元に適切な最適化シェーダーに置き換える
OptimizedShaderReplacer.Replace(replaceSettings);
```


