// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nova.Editor.Core.Scripts
{
    internal static class MaterialEditorUtility
    {
        private const int ProceduralTextureSize = 16;

        private static readonly Color HeaderBackgroundColorDark =
            new Color(40.0f / 256.0f, 40.0f / 256.0f, 40.0f / 256.0f);

        private static readonly Color ContentsBackgroundColorDark =
            new Color(48.0f / 256.0f, 48.0f / 256.0f, 48.0f / 256.0f);

        private static readonly Color BorderColorDark = new Color(16.0f / 256.0f, 16.0f / 256.0f, 16.0f / 256.0f);

        private static readonly Color HeaderBackgroundColorLight =
            new Color(170.0f / 256.0f, 170.0f / 256.0f, 170.0f / 256.0f);

        private static readonly Color ContentsBackgroundColorLight =
            new Color(193.0f / 256.0f, 193.0f / 256.0f, 193.0f / 256.0f);

        private static readonly Color BorderColorLight = new Color(101.0f / 256.0f, 101.0f / 256.0f, 101.0f / 256.0f);

        private static Texture2D _headerBackgroundTexture;
        private static Texture2D _contentsBackgroundTexture;
        private static GUIStyle _headerStyle;
        private static GUIStyle _contentsStyle;
        private static Texture _foldoutTrueIcon;
        private static Texture _foldoutFalseIcon;

        private static Color TextColor => EditorStyles.label.normal.textColor;

        private static Color HeaderBackgroundColor =>
            EditorGUIUtility.isProSkin ? HeaderBackgroundColorDark : HeaderBackgroundColorLight;

        private static Color ContentsBackgroundColor =>
            EditorGUIUtility.isProSkin ? ContentsBackgroundColorDark : ContentsBackgroundColorLight;

        private static Color BorderColor => EditorGUIUtility.isProSkin ? BorderColorDark : BorderColorLight;

        private static Texture2D HeaderBackgroundTexture
        {
            get
            {
                if (_headerBackgroundTexture == null)
                {
                    _headerBackgroundTexture = new Texture2D(ProceduralTextureSize, ProceduralTextureSize);
                    for (var i = 0; i < ProceduralTextureSize; i++)
                    for (var j = 0; j < ProceduralTextureSize; j++)
                    {
                        var isEdge = i == 0 || i == ProceduralTextureSize - 1 || j == 0 ||
                                     j == ProceduralTextureSize - 1;
                        var color = isEdge ? BorderColor : HeaderBackgroundColor;
                        _headerBackgroundTexture.SetPixel(i, j, color);
                    }

                    _headerBackgroundTexture.wrapMode = TextureWrapMode.Clamp;
                    _headerBackgroundTexture.filterMode = FilterMode.Point;
                    _headerBackgroundTexture.Apply();
                }

                return _headerBackgroundTexture;
            }
        }

        private static Texture2D ContentsBackgroundTexture
        {
            get
            {
                if (_contentsBackgroundTexture == null)
                {
                    _contentsBackgroundTexture = new Texture2D(ProceduralTextureSize, ProceduralTextureSize);
                    for (var i = 0; i < ProceduralTextureSize; i++)
                    for (var j = 0; j < ProceduralTextureSize; j++)
                    {
                        var isEdge = i == 0 || i == ProceduralTextureSize - 1 || j == 0;
                        var color = isEdge ? BorderColor : ContentsBackgroundColor;
                        _contentsBackgroundTexture.SetPixel(i, j, color);
                    }

                    _contentsBackgroundTexture.wrapMode = TextureWrapMode.Clamp;
                    _contentsBackgroundTexture.filterMode = FilterMode.Point;
                    _contentsBackgroundTexture.Apply();
                }

                return _contentsBackgroundTexture;
            }
        }

        private static GUIStyle HeaderStyle
        {
            get
            {
                if (_headerStyle == null || _headerBackgroundTexture == null)
                    _headerStyle = new GUIStyle
                    {
                        normal = new GUIStyleState
                        {
                            background = HeaderBackgroundTexture
                        },
                        border = new RectOffset(1, 1, 1, 1),
                        padding = new RectOffset(8, 8, 0, 4)
                    };

                return _headerStyle;
            }
        }

        private static GUIStyle ContentsStyle
        {
            get
            {
                if (_contentsStyle == null || _contentsBackgroundTexture == null)
                    _contentsStyle = new GUIStyle
                    {
                        normal = new GUIStyleState
                        {
                            background = ContentsBackgroundTexture
                        },
                        border = new RectOffset(1, 1, 1, 1),
                        padding = new RectOffset(8, 8, 0, 0),
                        margin = new RectOffset(0, 0, 0, 8)
                    };

                return _contentsStyle;
            }
        }

        /// <summary>
        ///     Draw a <see cref="Texture" /> type property with small icon.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="textureProp">
        ///     MaterialProperty of texture.Its type must be Texture, Texture2D or Texture 3D.
        ///     If the type is invalid, this function throws an exception.
        /// </param>
        /// <param name="channelsXProperty">
        ///     This property is used to select the texture channel to be used.
        ///     It can be null.
        /// </param>
        /// <param name="multipliedValueProp">
        ///     This property's value is multiplied by texture value.
        ///     It can be null.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void DrawSmallTexture(MaterialEditor editor, string label, MaterialProperty textureProp,
            MaterialProperty channelsXProperty, MaterialProperty multipliedValueProp)
        {
            // Texture
            Type textureType;
            switch (textureProp.textureDimension)
            {
                case TextureDimension.Unknown:
                case TextureDimension.None:
                case TextureDimension.Any:
                    textureType = typeof(Texture);
                    break;
                case TextureDimension.Tex2D:
                case TextureDimension.Cube:
                    textureType = typeof(Texture2D);
                    break;
                case TextureDimension.Tex3D:
                    textureType = typeof(Texture3D);
                    break;
                case TextureDimension.Tex2DArray:
                case TextureDimension.CubeArray:
                    textureType = typeof(Texture2DArray);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var mapHeight = EditorGUIUtility.singleLineHeight * 2;
            var mapWidth = mapHeight;

            var fullRect = EditorGUILayout.GetControlRect(false, mapHeight);
            var textureRect = fullRect;
            textureRect.width = mapHeight;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.showMixedValue = textureProp.hasMixedValue;
                var texture = (Texture)EditorGUI.ObjectField(textureRect, textureProp.textureValue,
                    textureType,
                    false);
                EditorGUI.showMixedValue = false;
                if (changeCheckScope.changed)
                {
                    editor.RegisterPropertyChangeUndo(textureProp.name);
                    textureProp.textureValue = texture;
                }
            }

            var offsetXFromTextureRectLeft = 8;
            var offsetXFromTextureRectRight = textureRect.width + offsetXFromTextureRectLeft;
            var offsetYFromTextureRectTop = 0.0f;
            if (multipliedValueProp == null)
                // If normalizedValueProp is null, num of property is 1.
                // Therefore, the coordinates of the properties are aligned to the center.  
                offsetYFromTextureRectTop = textureRect.height / 2 - EditorGUIUtility.singleLineHeight / 2;

            var propertyRect = EditorGUI.IndentedRect(fullRect);

            propertyRect.y += offsetYFromTextureRectTop;
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            if (multipliedValueProp != null)
            {
                editor.ShaderProperty(propertyRect, multipliedValueProp, label, 3);
            }
            else
            {
                var rect = propertyRect;
                rect.x += offsetXFromTextureRectRight;
                GUI.Label(rect, label);
            }

            propertyRect.xMin += offsetXFromTextureRectRight;
            propertyRect.y += EditorGUIUtility.singleLineHeight;
            if (channelsXProperty != null)
            {
                var xPropertyLabelRect = propertyRect;
                GUI.Label(xPropertyLabelRect, "Channels");

                var xPropertyXOffset = EditorGUIUtility.labelWidth - mapWidth - offsetXFromTextureRectLeft;
                var xPropertyRect = propertyRect;
                xPropertyRect.xMin += xPropertyXOffset;
                GUI.Label(xPropertyRect, "X");
                xPropertyRect.xMin += GUI.skin.label.fontSize;
                DrawEnumContentsProperty<ColorChannels>(editor, xPropertyRect,
                    channelsXProperty);
                propertyRect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        /// <summary>
        ///     Draw a <see cref="Texture" /> type property.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="textureProperty"></param>
        /// <param name="drawTilingAndOffset"></param>
        public static void DrawTexture<TCustomCoord>(MaterialEditor editor, MaterialProperty textureProperty,
            bool drawTilingAndOffset) where TCustomCoord : Enum
        {
            DrawTexture<TCustomCoord>(editor, textureProperty, drawTilingAndOffset, null, null, null, null);
        }

        /// <summary>
        ///     Draw a <see cref="Texture" /> type property with the custom coords for the offset.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="textureProperty"></param>
        /// <param name="offsetCoordXProperty"></param>
        /// <param name="offsetCoordYProperty"></param>
        /// <param name="channelsXProperty"></param>
        /// <param name="channelsYProperty"></param>
        public static void DrawTexture<TCustomCoord>(MaterialEditor editor, MaterialProperty textureProperty,
            MaterialProperty offsetCoordXProperty, MaterialProperty offsetCoordYProperty,
            MaterialProperty channelsXProperty, MaterialProperty channelsYProperty) where TCustomCoord : Enum
        {
            DrawTexture<TCustomCoord>(editor, textureProperty, true,
                offsetCoordXProperty, offsetCoordYProperty, channelsXProperty, channelsYProperty);
        }

        private static void DrawTexture<TCustomCoord>(MaterialEditor editor, MaterialProperty textureProperty,
            bool drawTilingAndOffset,
            MaterialProperty offsetCoordXProperty, MaterialProperty offsetCoordYProperty,
            MaterialProperty channelsXProperty, MaterialProperty channelsYProperty) where TCustomCoord : Enum
        {
            var propertyCount = 0;
            if (drawTilingAndOffset) propertyCount += 2;

            var useOffsetCoord = offsetCoordXProperty != null && offsetCoordYProperty != null;
            if (useOffsetCoord) propertyCount += 1;

            var useChannelsX = channelsXProperty != null;
            var useChannelsY = channelsYProperty != null;
            var useChannels = useChannelsX || useChannelsY;
            if (useChannels) propertyCount += 1;
            var contentsHeight = propertyCount * EditorGUIUtility.singleLineHeight +
                                 (propertyCount - 2) * EditorGUIUtility.standardVerticalSpacing;
            var fullHeight = Mathf.Max(contentsHeight + 8, 64);
            fullHeight += EditorGUIUtility.standardVerticalSpacing;

            var fullRect = EditorGUILayout.GetControlRect(false, fullHeight);

            var innerRect = fullRect;
            innerRect = EditorGUI.IndentedRect(innerRect);
            innerRect.height -= 8;
            innerRect.y += 4;

            var textureRect = innerRect;
            textureRect.height = 56;
            textureRect.width = textureRect.height;
            textureRect.y += (innerRect.height - textureRect.height) / 2;

            var contentsRect = fullRect;
            contentsRect = EditorGUI.IndentedRect(contentsRect);
            contentsRect.height = contentsHeight;
            contentsRect.y += (fullHeight - contentsHeight) / 2;
            contentsRect.xMin += textureRect.width + 4;

            var labelRect = contentsRect;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            labelRect.xMax -= fullRect.width - EditorGUIUtility.labelWidth;

            var propertyRect = contentsRect;
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            propertyRect.xMin += labelRect.width;

            // Draw properties.
            using (new ResetIndentLevelScope())
            {
                // Texture
                Type textureType;
                switch (textureProperty.textureDimension)
                {
                    case TextureDimension.Unknown:
                    case TextureDimension.None:
                    case TextureDimension.Any:
                        textureType = typeof(Texture);
                        break;
                    case TextureDimension.Tex2D:
                    case TextureDimension.Cube:
                        textureType = typeof(Texture2D);
                        break;
                    case TextureDimension.Tex3D:
                        textureType = typeof(Texture3D);
                        break;
                    case TextureDimension.Tex2DArray:
                    case TextureDimension.CubeArray:
                        textureType = typeof(Texture2DArray);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.showMixedValue = textureProperty.hasMixedValue;
                    var texture = (Texture)EditorGUI.ObjectField(textureRect, textureProperty.textureValue, textureType,
                        false);
                    EditorGUI.showMixedValue = false;
                    if (changeCheckScope.changed)
                    {
                        editor.RegisterPropertyChangeUndo(textureProperty.name);
                        textureProperty.textureValue = texture;
                    }
                }

                // Labels
                if (drawTilingAndOffset)
                {
                    GUI.Label(labelRect, "Tiling");
                    labelRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    GUI.Label(labelRect, "Offset");
                }

                if (useOffsetCoord)
                {
                    labelRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    GUI.Label(labelRect, "Offset Coords");
                }

                if (useChannels)
                {
                    labelRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    GUI.Label(labelRect, "Channels");
                }

                // Tiling & Offsets
                if (drawTilingAndOffset)
                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        var textureScaleAndOffset = textureProperty.textureScaleAndOffset;
                        var tiling = new Vector2(textureScaleAndOffset.x, textureScaleAndOffset.y);
                        var offset = new Vector2(textureScaleAndOffset.z, textureScaleAndOffset.w);
                        tiling = EditorGUI.Vector2Field(propertyRect, string.Empty, tiling);
                        propertyRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        offset = EditorGUI.Vector2Field(propertyRect, string.Empty, offset);
                        if (changeCheckScope.changed)
                        {
                            textureScaleAndOffset = new Vector4(tiling.x, tiling.y, offset.x, offset.y);
                            editor.RegisterPropertyChangeUndo(textureProperty.name);
                            textureProperty.textureScaleAndOffset = textureScaleAndOffset;
                        }
                    }

                // Offset Coords
                if (useOffsetCoord)
                {
                    propertyRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    var xRect = propertyRect;
                    xRect.width /= 2;
                    var xPropertyRect = xRect;
                    xPropertyRect.xMin += 12;
                    EditorGUI.LabelField(xRect, new GUIContent("X"));
                    DrawEnumContentsProperty<TCustomCoord>(editor, xPropertyRect, offsetCoordXProperty);

                    var yRect = xRect;
                    yRect.x += yRect.width + 2;
                    yRect.xMax -= 2;
                    var yPropertyRect = yRect;
                    yPropertyRect.xMin += 12;
                    EditorGUI.LabelField(yRect, new GUIContent("Y"));
                    DrawEnumContentsProperty<TCustomCoord>(editor, yPropertyRect, offsetCoordYProperty);
                }

                // Channels
                if (useChannels)
                {
                    propertyRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    var xRect = propertyRect;
                    if (useChannelsY) xRect.width /= 2;

                    var xPropertyRect = xRect;
                    xPropertyRect.xMin += 12;
                    EditorGUI.LabelField(xRect, new GUIContent("X"));
                    DrawEnumContentsProperty<ColorChannels>(editor, xPropertyRect, channelsXProperty);

                    if (useChannelsY)
                    {
                        var yRect = xRect;
                        yRect.x += yRect.width + 2;
                        yRect.xMax -= 2;
                        var yPropertyRect = yRect;
                        yPropertyRect.xMin += 12;
                        EditorGUI.LabelField(yRect, new GUIContent("Y"));
                        DrawEnumContentsProperty<ColorChannels>(editor, yPropertyRect, channelsYProperty);
                    }
                }
            }
        }

        /// <summary>
        ///     Draw a property with custom coord for it.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <param name="coordProperty"></param>
        public static void DrawPropertyAndCustomCoord<T>(MaterialEditor editor, string label, MaterialProperty property,
            MaterialProperty coordProperty) where T : Enum
        {
            var fullRect = EditorGUILayout.GetControlRect();
            var contentsRect = fullRect;
            contentsRect.xMin += EditorGUIUtility.labelWidth;
            var propertyRect = fullRect;
            propertyRect.xMax -= contentsRect.width / 4;
            var coordRect = contentsRect;
            coordRect.width -= contentsRect.width * 3 / 4 - 2;
            coordRect.x += contentsRect.width * 3 / 4 + 2;

            editor.ShaderProperty(propertyRect, property, label);

            using (new ResetIndentLevelScope())
            {
                T coord = (T)Enum.ToObject(typeof(T), Convert.ToInt32(coordProperty.floatValue));
                if (!Enum.IsDefined(typeof(T), coord))
                {
                    EditorGUILayout.HelpBox(
                        $"Invalid coord value\n", MessageType.Error, true);
                }
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.showMixedValue = coordProperty.hasMixedValue;
                    coord = (T)EditorGUI.EnumPopup(coordRect, coord);
                    EditorGUI.showMixedValue = false;

                    if (ccs.changed)
                    {
                        editor.RegisterPropertyChangeUndo(coordProperty.name);
                        coordProperty.floatValue = Convert.ToInt32(coord);
                    }
                }
            }
        }

        /// <summary>
        ///     Draw a int type property with the slider.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void DrawIntRangeProperty(MaterialEditor editor, string label, MaterialProperty property, int min,
            int max)
        {
            DrawProperty(label, rect =>
            {
                var value = (int)property.floatValue;
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.showMixedValue = property.hasMixedValue;
                    value = EditorGUI.IntSlider(rect, value, min, max);
                    EditorGUI.showMixedValue = false;

                    if (ccs.changed)
                    {
                        editor.RegisterPropertyChangeUndo(property.name);
                        property.floatValue = value;
                    }
                }
            });
        }
        
        /// <summary>
        /// Draw a float type property with the slider.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void DrawFloatRangeProperty(MaterialEditor editor, string label, MaterialProperty property, 
            float min,
            float max)
        {
            DrawProperty(label, rect =>
            {
                var value = property.floatValue;
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.showMixedValue = property.hasMixedValue;
                    value = EditorGUI.Slider(rect, value, min, max);
                    EditorGUI.showMixedValue = false;

                    if (ccs.changed)
                    {
                        editor.RegisterPropertyChangeUndo(property.name);
                        property.floatValue = value;
                    }
                }
            });
        }

        /// <summary>
        ///     Draw a enum type property.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <typeparam name="T"></typeparam>
        public static void DrawEnumProperty<T>(MaterialEditor editor, string label, MaterialProperty property)
            where T : Enum
        {
            DrawProperty(label, rect => { DrawEnumContentsProperty<T>(editor, rect, property); });
        }

        /// <summary>
        ///     Draw a enum flags type property.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="property"></param>
        /// <typeparam name="T"></typeparam>
        public static void DrawEnumFlagsProperty<T>(MaterialEditor editor, string label, MaterialProperty property)
            where T : Enum
        {
            DrawProperty(label, rect => { DrawEnumFlagsContentsProperty<T>(editor, rect, property); });
        }

        /// <summary>
        ///     Draw a bool type property with toggle GUI.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="property"></param>
        public static void DrawToggleProperty(MaterialEditor editor, string label, MaterialProperty property)
        {
            DrawProperty(label, rect =>
            {
                var isOn = property.floatValue >= 0.5f;

                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.showMixedValue = property.hasMixedValue;
                    isOn = EditorGUI.Toggle(rect, isOn);
                    EditorGUI.showMixedValue = false;
                    if (ccs.changed)
                    {
                        editor.RegisterPropertyChangeUndo(property.name);
                        property.floatValue = isOn ? 1.0f : 0.0f;
                    }
                }
            });
        }

        /// <summary>
        ///     Draw a <see cref="Vector2" /> type property.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="label"></param>
        /// <param name="property"></param>
        public static void DrawVector2Property(MaterialEditor editor, string label, MaterialProperty property)
        {
            DrawProperty(label, rect =>
            {
                var value = (Vector2)property.vectorValue;

                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.showMixedValue = property.hasMixedValue;
                    value = EditorGUI.Vector2Field(rect, string.Empty, value);
                    EditorGUI.showMixedValue = false;

                    if (ccs.changed)
                    {
                        editor.RegisterPropertyChangeUndo(property.name);
                        property.vectorValue = value;
                    }
                }
            });
        }

        /// <summary>
        ///     Draw two float type properties.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="prop1"></param>
        /// <param name="prop1Label"></param>
        /// <param name="prop2"></param>
        /// <param name="prop2Label"></param>
        /// <param name="materialEditor"></param>
        /// <param name="propLabelWidth"></param>
        public static void DrawTwoFloatProperties(string title, MaterialProperty prop1, string prop1Label,
            MaterialProperty prop2, string prop2Label, MaterialEditor materialEditor, float propLabelWidth = 30f)
        {
            EditorGUI.showMixedValue = prop1.hasMixedValue || prop2.hasMixedValue;
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.PrefixLabel(rect, new GUIContent(title));
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;

            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                EditorGUIUtility.labelWidth = propLabelWidth;
                var prop1Rect = new Rect(rect.x + labelWidth, rect.y, (rect.width - labelWidth) * 0.5f,
                    EditorGUIUtility.singleLineHeight);
                var prop1Value = EditorGUI.FloatField(prop1Rect, prop1Label, prop1.floatValue);

                var prop2Rect = new Rect(prop1Rect.x + prop1Rect.width, rect.y,
                    prop1Rect.width, EditorGUIUtility.singleLineHeight);
                prop2Rect.xMin += 4;
                var prop2Value = EditorGUI.FloatField(prop2Rect, prop2Label, prop2.floatValue);
                EditorGUIUtility.labelWidth = labelWidth;

                if (ccs.changed)
                {
                    materialEditor.RegisterPropertyChangeUndo(title);
                    prop1.floatValue = prop1Value;
                    prop2.floatValue = prop2Value;
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.showMixedValue = false;
        }

        public static void DrawEnumContentsProperty<T>(MaterialEditor editor, Rect rect, MaterialProperty property)
            where T : Enum
        {
            var value = (T)Enum.ToObject(typeof(T), (int)property.floatValue);
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.showMixedValue = property.hasMixedValue; 
                if (!Enum.IsDefined(typeof(T), value))
                {
                    EditorGUILayout.HelpBox(
                        $"Invalid coord value\n", MessageType.Error, true);
                }
                
                var intValue = Convert.ToInt32(EditorGUI.EnumPopup(rect, value));
                
                
                EditorGUI.showMixedValue = false;

                if (ccs.changed)
                {
                    editor.RegisterPropertyChangeUndo(property.name);
                    property.floatValue = intValue;
                }
            }
        }

        private static void DrawEnumFlagsContentsProperty<T>(MaterialEditor editor, Rect rect,
            MaterialProperty property)
            where T : Enum
        {
            var value = (T)Enum.ToObject(typeof(T), (int)property.floatValue);
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                EditorGUI.showMixedValue = property.hasMixedValue;

                var intValue = Convert.ToInt32(EditorGUI.EnumFlagsField(rect, value));
                EditorGUI.showMixedValue = false;

                if (ccs.changed)
                {
                    editor.RegisterPropertyChangeUndo(property.name);
                    property.floatValue = intValue;
                }
            }
        }

        private static void DrawProperty(string label, Action<Rect> drawContents)
        {
            var fullRect = EditorGUILayout.GetControlRect();
            var contentsRect = EditorGUI.PrefixLabel(fullRect, new GUIContent(label));
            contentsRect.xMin -= 2;

            using (new ResetIndentLevelScope())
            {
                drawContents.Invoke(contentsRect);
            }
        }

        public static void SetKeyword(Material material, string keyword, bool state)
        {
            if (state)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        private static Texture GetFoldoutTrueIcon()
        {
            if (_foldoutTrueIcon == null) _foldoutTrueIcon = Resources.Load<Texture>("tex_editor_icon_foldout_true");

            return _foldoutTrueIcon;
        }

        private static Texture GetFoldoutFalseIcon()
        {
            if (_foldoutFalseIcon == null) _foldoutFalseIcon = Resources.Load<Texture>("tex_editor_icon_foldout_false");

            return _foldoutFalseIcon;
        }

        /// <summary>
        ///     Set the indent level to zero and determine its scope with the using state.
        /// </summary>
        public class ResetIndentLevelScope : GUI.Scope
        {
            private readonly int _indentLevel;

            public ResetIndentLevelScope()
            {
                _indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
            }

            protected override void CloseScope()
            {
                EditorGUI.indentLevel = _indentLevel;
            }
        }

        /// <summary>
        ///     Draw a foldout header and determine its scope with the using state.
        /// </summary>
        public class FoldoutHeaderScope : GUI.Scope
        {
            private static GUIStyle _textStyle;

            public FoldoutHeaderScope(bool foldout, string text)
            {
                Foldout = foldout;
                EditorGUILayout.BeginVertical(HeaderStyle);
                var rect = EditorGUILayout.GetControlRect();
                var iconRect = rect;
                var height = iconRect.height;
                iconRect.width = 9;
                iconRect.height = 9;
                iconRect.y += (height - iconRect.height) / 2;

                var buttonRect = rect;
                buttonRect.y -= EditorGUIUtility.standardVerticalSpacing;
                buttonRect.height += EditorGUIUtility.standardVerticalSpacing * 2.0f;

                var textRect = rect;
                textRect.xMin += 14;

                var iconImage = foldout ? GetFoldoutTrueIcon() : GetFoldoutFalseIcon();
                GUI.DrawTexture(iconRect, iconImage, ScaleMode.ScaleToFit);
                if (_textStyle == null)
                    _textStyle = new GUIStyle
                    {
                        padding = new RectOffset(0, 0, 2, 0),
                        normal =
                        {
                            textColor = TextColor
                        },
                        fontStyle = FontStyle.Bold
                    };

                GUI.Label(textRect, text, _textStyle);
                if (GUI.Button(buttonRect, string.Empty, _textStyle)) Foldout = !Foldout;

                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(ContentsStyle);
                if (Foldout) GUILayout.Space(6);
            }

            public bool Foldout { get; }

            protected override void CloseScope()
            {
                // When not in foldout state, this space must be greater than or equal to 12 because the display will collapse.
                var space = Foldout ? 6 : 12;
                GUILayout.Space(space);
                EditorGUILayout.EndVertical();
            }
        }
    }
}
