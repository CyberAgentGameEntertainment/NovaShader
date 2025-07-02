# OptimizedShaderGenerator クラス
UberUnlit/Litシェーダーからシェーダーキーワード、パスを必要最小限にした、最適化されたシェーダーを生成する機能を提供するクラスです。<br/>

このクラスと併用してOptimizedShaderReplacerを利用すると、UberUnlit/UberLitを利用しているマテリアルのシェーダーを最適化シェーダーに差し替えることができます。

## APIs
- [Generate](#optimizedshadergenerator-クラス)
  
## Generate
### 概要
UberLitとUberUnlitシェーダーを最適化し、指定された出力フォルダに保存します。

### 引数
|パラメータ|説明|
|---|---|
|outputFolderPath|最適化されたシェーダーを出力するフォルダのパス|

### サンプルコード
```C#
OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
```

## FixErrorWithMaterial
### 概要
指定されたマテリアルを使用するParticleSystemRendereのエラーを修正します。<br/>

### 引数
|パラメータ|説明|
|---|---|
|material|ParticleSystemRendererに指定されているマテリアル|

### 備考
どのParticleSystemRendererにもマテリアルが指定されていない場合はこのメソッドは何もしません。

