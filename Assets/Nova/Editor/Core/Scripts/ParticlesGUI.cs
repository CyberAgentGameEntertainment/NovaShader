// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Core.Scripts
{
    public abstract class ParticlesGUI : ShaderGUI
    {
        private bool _isInitialized;

        public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
        {
            // Find properties every time to update the inspector when undo or redo is performed.
            SetupProperties(properties);

            if (!_isInitialized)
            {
                Initialize(editor, properties);

                foreach (var obj in editor.targets)
                {
                    MaterialChanged((Material)obj);
                }

                _isInitialized = true;
            }

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                DrawGUI(editor, properties);

                if (changeCheckScope.changed)
                {
                    foreach (var obj in editor.targets)
                    {
                        MaterialChanged((Material)obj);
                    }
                }
            }
        }

        protected abstract void SetupProperties(MaterialProperty[] properties);

        protected abstract void Initialize(MaterialEditor editor, MaterialProperty[] properties);

        protected abstract void DrawGUI(MaterialEditor editor, MaterialProperty[] properties);

        protected abstract void MaterialChanged(Material material);

        protected class Property
        {
            public Property(string name)
            {
                Name = name;
            }

            public string Name { get; }
            public MaterialProperty Value { get; private set; }

            public void Setup(MaterialProperty[] properties)
            {
                Value = FindProperty(Name, properties);
            }
        }
    }
}