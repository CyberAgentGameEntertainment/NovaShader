# OptimizedShaderReplacer クラス
UberUnlit/Litが割り当てられているマテリアルのシェーダーを設定に応じて最適化シェーダーに置き換える機能を提供するクラスです。<br/>
なお、置き換えられる最適化シェーダーは[OptimizedShaderGeneratorクラス](OptimizedShaderGenerator_JA.md)で事前に作成されている必要があります。

OptimizedShaderReplacerとOptimizedShaderGeneratorを併用するサンプルコードは`Assets/Samples/Editor/ShaderOptimizeSample.cs`に記載されているので、こちらも参照してください。

## APIs
- [Replace](#Replace)

## Replace
### 概要
UberUnlit/Litシェーダーを使用しているマテリアルを、最適化されたシェーダーに置き換えます。

### 引数
|パラメータ|説明|
|---|---|
|settings|シェーダー置換のパラメータ。<br/>詳細は[OptimizedShaderReplacer.Settings](OptimizedShaderReplacer_Settings_JA.md)を参照して下さい。|


### サンプルコード
```C#
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

### 備考
シェーダーの差し替え設定（OptimizedShaderReplacer.Settings）はプロジェクトの状況に応じて適切に設定を行ってください。<br/>
サンプルコードではDepthPrepassが実行されるため`DepthOnly`パスを設定に加えていますが、DepthPrepassが実行されないのであれば、削除することが可能です。<br/>
これらの設定はプロジェクトの状況に応じて変わるため適切な最適化シェーダーが割り当てられるように慎重に検討してください。<br/>
（ただし、メモリ使用量の観点からUberシェーダーより悪化することはありません。）


