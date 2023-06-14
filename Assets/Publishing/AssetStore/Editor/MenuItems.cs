using UnityEditor;

namespace Publishing.AssetStore.Editor
{
    internal static class MenuItems
    {
        [MenuItem("Tools/NOVA Shader/Asset Store/Preprocess")]
        private static void PreprocessAssetStorePublishing()
        {
            try
            {
                var organizeHierarchyService = new OrganizeHierarchyService();
                organizeHierarchyService.Execute();

                var modifyReadmeService = new ModifyReadmeService();
                modifyReadmeService.Execute();
            }
            catch
            {
                EditorUtility.DisplayDialog("Error",
                    "Error occurred while organizing file hierarchy.\nSee console for details.",
                    "OK");
                throw;
            }
        }
    }
}