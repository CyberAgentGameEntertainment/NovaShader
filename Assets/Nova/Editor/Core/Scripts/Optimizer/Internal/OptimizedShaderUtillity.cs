using System.Collections.Generic;

namespace Nova.Editor.Core.Scripts.Optimizer.Internal
{
    internal static class OptimizedShaderUtillity
    {
        private static string GetOptimizedShaderPostFixName(RenderType renderType, OptionalShaderPass requiredPasses)
        {
            var requiredOptionalPassName = "";
            if(requiredPasses == OptionalShaderPass.None)
            {
                requiredOptionalPassName = "None";
            }else
            {
                var optionalPassNames = new List<string>();
                if((requiredPasses & OptionalShaderPass.DepthOnly) != 0)
                {
                    optionalPassNames.Add("DepthOnly");
                }
                if((requiredPasses & OptionalShaderPass.DepthNormals) != 0)
                {
                    optionalPassNames.Add("DepthNormals");
                }
                if((requiredPasses & OptionalShaderPass.ShadowCaster) != 0)
                {
                    optionalPassNames.Add("ShadowCaster");
                }
                requiredOptionalPassName = string.Join(" ", optionalPassNames);
            }
            
            var renderTypeName = "";
            if(renderType == RenderType.Opaque)
            {
                renderTypeName = "Opaque";
            }
            else if(renderType == RenderType.Transparent)
            {
                renderTypeName = "Transparent";
            }
            else if(renderType == RenderType.Cutout)
            {
                renderTypeName = "Cutout";
            }

            return $"(Optimized {renderTypeName} {requiredOptionalPassName})";
        }
        public static string GetOptimizedShaderName(string baseName, RenderType renderType, OptionalShaderPass requiredPasses)
        {
            var postFixName = GetOptimizedShaderPostFixName(renderType, requiredPasses);
            return $"Hidden/{baseName}{postFixName}";
        }
    }
}
