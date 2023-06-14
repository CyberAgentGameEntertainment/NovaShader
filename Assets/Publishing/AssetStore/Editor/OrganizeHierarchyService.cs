using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Publishing.AssetStore.Editor
{
    internal sealed class OrganizeHierarchyService
    {
        public void Execute()
        {
            // Move README.md, README_JA.md, Third Party Notices.md, LICENSE.md, Documentation to Assets/Nova
            MoveProjectFile(Paths.ReadmeFileName, $"{Paths.OldRootFolderAssetPath}/{Paths.ReadmeFileName}");
            MoveProjectFile(Paths.JapaneseReadmeFileName, $"{Paths.OldRootFolderAssetPath}/{Paths.JapaneseReadmeFileName}");
            MoveProjectFile(Paths.ThirdPartyNoticesFileName,
                $"{Paths.OldRootFolderAssetPath}/{Paths.ThirdPartyNoticesFileName}");
            MoveProjectFile(Paths.LicenseFileName, $"{Paths.OldRootFolderAssetPath}/{Paths.LicenseFileName}");
            MoveProjectFolder(Paths.DocumentationFolderName, $"{Paths.OldRootFolderAssetPath}/{Paths.DocumentationFolderName}");
            AssetDatabase.Refresh();

            // Assets/Nova -> Assets/Nova Shader
            var error = AssetDatabase.MoveAsset(Paths.OldRootFolderAssetPath, Paths.NewRootFolderAssetPath);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            // Create Assets/Nova Shader/Demo & Samples
            AssetDatabase.CreateFolder(Paths.NewRootFolderAssetPath, Paths.NewDemoSamplesFolderName);

            var demoSamplesFolderAssetPath = $"{Paths.NewRootFolderAssetPath}/{Paths.NewDemoSamplesFolderName}";

            // Assets/Demo -> Assets/Nova Shader/Demo & Samples/Demo
            var oldDemoFolderAssetPath = $"{Paths.OldDemoSamplesRootFolderAssetPath}/{Paths.DemoFolderName}";
            MoveToFolder(oldDemoFolderAssetPath, demoSamplesFolderAssetPath);

            // Assets/Samples -> Assets/Nova Shader/Demo & Samples/Samples
            var oldSamplesFolderAssetPath = $"{Paths.OldDemoSamplesRootFolderAssetPath}/{Paths.SamplesFolderName}";
            MoveToFolder(oldSamplesFolderAssetPath, demoSamplesFolderAssetPath);

            // Assets/Settings -> Assets/Nova Shader/Settings
            var oldSettingsFolderAssetPath = $"{Paths.OldDemoSamplesRootFolderAssetPath}/{Paths.SettingsFolderName}";
            MoveToFolder(oldSettingsFolderAssetPath, demoSamplesFolderAssetPath);
        }

        private static void MoveToFolder(string assetPath, string newFolderAssetPath)
        {
            var newAssetPath = $"{newFolderAssetPath}/{Path.GetFileName(assetPath)}";
            var error = AssetDatabase.MoveAsset(assetPath, newAssetPath);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);
        }

        private static void MoveProjectFile(string oldRelativePath, string newRelativePath)
        {
            var projectFolderPath = Path.GetDirectoryName(Application.dataPath);
            var oldPath = Path.Combine(projectFolderPath, oldRelativePath);
            var newPath = Path.Combine(projectFolderPath, newRelativePath);
            File.Move(oldPath, newPath);
        }
        
        private static void MoveProjectFolder(string oldRelativePath, string newRelativePath)
        {
            var projectFolderPath = Path.GetDirectoryName(Application.dataPath);
            var oldPath = Path.Combine(projectFolderPath, oldRelativePath);
            var newPath = Path.Combine(projectFolderPath, newRelativePath);
            Directory.Move(oldPath, newPath);
        }
    }
}