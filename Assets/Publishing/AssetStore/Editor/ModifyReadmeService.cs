using System.IO;

namespace Publishing.AssetStore.Editor
{
    internal sealed class ModifyReadmeService
    {
        public void Execute()
        {
            // README
            var readmeFilePath = Path.Combine(Paths.NewRootFolderAssetPath, Paths.ReadmeFileName);
            var text = File.ReadAllText(readmeFilePath);
            text = ModifyContents(text);
            File.WriteAllText(readmeFilePath, text);

            // README_JA
            var japaneseReadmeFilePath = Path.Combine(Paths.NewRootFolderAssetPath, Paths.JapaneseReadmeFileName);
            text = File.ReadAllText(japaneseReadmeFilePath);
            text = ModifyContents(text);
            File.WriteAllText(japaneseReadmeFilePath, text);
        }

        private string ModifyContents(string text)
        {
            // Assets/Demo -> Assets/Nova Shader/Demo & Samples/Demo
            var oldDemoRelativePath =
                ModifyLinkText(Path.Combine(Paths.OldDemoSamplesRootFolderAssetPath, Paths.DemoFolderName));
            var newDemoRelativePath =
                ModifyLinkText(Path.Combine(Paths.NewDemoSamplesFolderName, Paths.DemoFolderName));
            text = text.Replace(oldDemoRelativePath, newDemoRelativePath);

            // Assets/Samples -> Assets/Nova Shader/Demo & Samples/Samples
            var oldSamplesRelativePath =
                ModifyLinkText(Path.Combine(Paths.OldDemoSamplesRootFolderAssetPath, Paths.SamplesFolderName));
            var newSamplesRelativePath =
                ModifyLinkText(Path.Combine(Paths.NewDemoSamplesFolderName, Paths.SamplesFolderName));
            text = text.Replace(oldSamplesRelativePath, newSamplesRelativePath);

            return text;
        }

        private string ModifyLinkText(string linkText)
        {
            linkText = linkText.Replace(" ", "%20");
            return linkText;
        }
    }
}