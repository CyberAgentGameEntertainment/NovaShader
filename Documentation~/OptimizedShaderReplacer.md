# OptimizedShaderReplacer Class
A class that provides functionality to replace shaders in materials using UberUnlit/Lit with optimized shaders based on settings.<br/>
Note that the optimized shaders to be replaced must be created in advance using the [OptimizedShaderGenerator class](OptimizedShaderGenerator.md).
For sample code demonstrating combined usage of OptimizedShaderReplacer and OptimizedShaderGenerator, please refer to `Assets/Samples/Editor/ShaderOptimizeSample.cs`.
## APIs
- [Replace](#Replace)

## Replace
### Overview
Replaces materials using UberUnlit/Lit shaders with optimized shaders.

### Parameters
|Parameter|Description|
|---|---|
|settings|Parameters for shader replacement.<br/>For details, see [OptimizedShaderReplacer.Settings](OptimizedShaderReplacer_Settings.md).|


### Sample Code
```C#
// Configure shader replacement settings
var replaceSettings = new OptimizedShaderReplacer.Settings
{
    // DepthOnly is required for opaque rendering because Depth Prepass is executed.
    // ShadowCaster is also required for shadow casting.
    OpaqueRequiredPasses = OptionalShaderPass.DepthOnly | OptionalShaderPass.ShadowCaster,
    CutoutRequiredPasses = OptionalShaderPass.DepthOnly|OptionalShaderPass.ShadowCaster,
    // DepthOnly is not required for transparent rendering since depth values are not written in Depth Prepass.
    // Optional passes are not needed since shadow casting is not performed either.
    TransparentRequiredPasses = OptionalShaderPass.None
};
// Replace with appropriate optimized shaders based on settings
OptimizedShaderReplacer.Replace(replaceSettings);
```

### 備考
Please configure shader replacement settings (OptimizedShaderReplacer.Settings) appropriately according to your project's requirements.<br/>
In the sample code, the `DepthOnly` pass is included in the settings because Depth Prepass is executed, but if Depth Prepass is not executed, it can be removed.<br/>
Since these settings vary depending on your project's conditions, please carefully consider them to ensure appropriate optimized shaders are assigned.<br/>
(However, from a memory usage perspective, it will never be worse than using Uber shaders.)