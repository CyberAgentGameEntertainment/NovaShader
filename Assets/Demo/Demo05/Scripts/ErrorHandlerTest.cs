// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using Nova.Editor.Core.Scripts;
using UnityEditor;
using UnityEngine;

namespace Demo.Demo05.Scripts
{
    [CustomEditor(typeof(ErrorHandlerTest))]
    public class ErrorHandlerTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var serializedObject = new SerializedObject(target);
            var serializedProperty = serializedObject.FindProperty("material");
            var material = serializedProperty.objectReferenceValue as Material;
            if (GUILayout.Button("Fix Now")) RendererErrorHandler.FixErrorWithMaterial(material);

            if (GUILayout.Button("Check Error"))
            {
                if (RendererErrorHandler.CheckErrorWithMaterial(material))
                    Debug.Log("Errors");
                else
                    Debug.Log("No Errors    ");
            }
        }
    }

    public class ErrorHandlerTest : MonoBehaviour
    {
        [SerializeField] private Material material;
    }
}