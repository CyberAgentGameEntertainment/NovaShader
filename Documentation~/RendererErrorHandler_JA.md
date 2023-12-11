# Render Error Handler クラス
レンダリングに関するエラーハンドリングを提供するユーティリティです。
## CheckErrorWithMaterial メソッド
### 概要
指定されたマテリアルを使用するパーティクルシステムレンダラーのエラーをチェックします。

### 引数
|パラメータ|説明|
|---|---|
|material|ParticleSystemRendererに指定されているマテリアル|

### 戻り値
エラーがある場合はtrueを返します。<br/>

### サンプルコード
```C#
if (RendererErrorHandler.CheckErrorWithMaterial(material))
    Debug.Log("Errors");
else
    Debug.Log("No Errors");
```
## FixErrorWithMaterial メソッド
### 概要
指定されたマテリアルを使用するParticleSystemRendereのエラーを修正します。<br/>

### 引数
|パラメータ|説明|
|---|---|
|material|ParticleSystemRendererに指定されているマテリアル|

### 備考
どのParticleSystemRendererにもマテリアルが指定されていない場合はこのメソッドは何もしません。

