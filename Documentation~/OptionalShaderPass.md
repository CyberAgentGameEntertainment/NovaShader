# OptionalShaderPass
An enumeration type representing optional shader passes.

## Values
|Value|Description|
|---|---|
|None|No optional pass|
|DepthOnly|A pass that renders only depth values.<br/>Required when Depth Prepass is enabled.|
|DepthNormals|A pass that renders depth values and normals.<br/>Required when Depth Normals Prepass is enabled.|
|ShadowCaster|A pass that renders shadow maps.<br/>Required when shadow casting is enabled.|

## Example Usage
```cs
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


