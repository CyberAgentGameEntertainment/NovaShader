# OptimizedShaderReplacer.Settings Class
Settings class for shader replacement

## Properties
|Property Name|Description|
|---|---|
|OpaqueRequiredPasses|Optional passes required for opaque rendering.<br/>Specify a combination of OptionalShaderPass.<br/>For details, see [OptionalShaderPass](OptionalShaderPass.md).<br/>|
|TransparentRequiredPasses|Optional passes required for transparent rendering.<br/>Specify a combination of OptionalShaderPass.<br/>For details, see [OptionalShaderPass](OptionalShaderPass.md).|
|CutoutRequiredPasses|Optional passes required for cutout rendering.<br/>Specify a combination of OptionalShaderPass.<br/>For details, see [OptionalShaderPass](OptionalShaderPass.md).|
|TargetFolderPath|Root path of the folder containing materials to be replaced.<br/>If not specified, all materials in the project will be targeted.|



