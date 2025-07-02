# OptimizedShaderGenerator Class
A class that generates optimized shaders from UberUnlit/Lit shaders by minimizing shader keywords and passes.<br/>

Generated shaders are created with the following combinations:
- Rendering types (Opaque, Transparent, Cutout)
- Optional passes (DepthOnly, DepthNormals, ShadowCaster)
  
Using the [OptimizedShaderReplacer class](OptimizedShaderReplacer.md), shaders in materials using UberUnlit/UberLit can be replaced with the created optimized shaders.<br/>

For specific usage examples, please refer to the sample code in `Assets/Samples/Editor/ShaderOptimizeSample.cs`.
This sample demonstrates an implementation example combining OptimizedShaderGenerator and OptimizedShaderReplacer.

## APIs
- [Generate](#optimizedshadergenerator-class)
  
## Generate
### Overview
Optimizes UberLit and UberUnlit shaders and saves them to the specified output folder.

### Parameters
|Parameter|Description|
|---|---|
|outputFolderPath|Path to the folder where optimized shaders will be output|

### Sample Code
```C#
// Output optimized shaders to the specified folder
OptimizedShaderGenerator.Generate("Assets/OptimizedShaders");
```


