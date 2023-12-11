# RenderErrorHandler Class
Utilities that provide error handling for rendering.
## APIs
- [CheckErrorWithMaterial](#CheckErrorWithMaterial)
- [FixErrorWithMaterial](#fixerrorwithmaterial)
## CheckErrorWithMaterial
### Summary
Checks for errors in the Particle System Renderers using the specified material.
### Parameters
|Param|Description|
|---|---|
|material|Material specified for the Particle System Renderers.|

### Return
Returns true if an error has occurred.<br/>


### Code
```C#
if (RendererErrorHandler.CheckErrorWithMaterial(material))
    Debug.Log("Errors");
else
    Debug.Log("No Errors");
```
## FixErrorWithMaterial
### Summary
Corrects errors in the Particle System Renderers using the specified material.<br/>

### Parameters
|Param|Description|
|---|---|
|material|Material specified for the Particle System Renderers.|

### Remark
Ignored if the material is not specified in the Particle System Renderers.

