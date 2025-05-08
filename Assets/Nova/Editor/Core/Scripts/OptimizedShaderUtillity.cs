namespace Nova.Editor.Core.Scripts
{
    internal static class OptimizedShaderUtillity
    {
        public static string GetOptimizedShaderPostFixName(RenderType renderType, OptionalShaderPass requiredPasses)
        {
            var requiredOptionalPassName = "";
            if(requiredPasses == OptionalShaderPass.None)
            {
                requiredOptionalPassName = "None";
            }else
            {
                if((requiredPasses & OptionalShaderPass.DepthOnly) != 0)
                {
                    requiredOptionalPassName = "DepthOnly";
                }
                if((requiredPasses & OptionalShaderPass.DepthNormals) != 0)
                {
                    requiredOptionalPassName += " DepthNormals";
                }
                if((requiredPasses & OptionalShaderPass.ShadowCaster) != 0)
                {
                    requiredOptionalPassName += " ShadowCaster";
                }
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
