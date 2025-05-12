# OptimizedShaderGenerator クラス
UberUnlit/Litシェーダーからシェーダーキーワード、パスを必要最小限にした、最適化されたシェーダーを生成するクラスです。<br/>

生成されるシェーダーは以下の組み合わせで作成されます。
- レンダリングタイプ (Opaque、Transparent、Cutout)
- オプショナルパス (DepthOnly、DepthNormals、ShadowCaster)
  
生成されたシェーダーは[OptimizedShaderReplacerクラス](OptimizedShaderReplacer_JA.md)を利用することでUberUnlit/UberLitを利用しているマテリアルのシェーダーを作成した最適化シェーダーに差し替えることができます。<br/>

具体的な使用方法については、`Assets/Samples/Editor/ShaderOptimizeSample.cs`のサンプルコードをご参照ください。
このサンプルでは、OptimizedShaderGeneratorとOptimizedShaderReplacerを組み合わせた実装例を確認できます。


## APIs
  - [Generate](#generate)
  
## Generate
### 概要
UberLitとUberUnlitシェーダーを最適化し、指定された出力フォルダに保存します。

### 引数
|パラメータ|説明|
|---|---|
|outputFolderPath|最適化されたシェーダーを出力するフォルダのパス|

### サンプルコード
```C#
// 最適化されたシェーダーを指定したフォルダに出力する
OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
```



