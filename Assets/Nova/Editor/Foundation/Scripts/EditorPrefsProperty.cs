// --------------------------------------------------------------
// Copyright 2021 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEditor;

namespace Nova.Editor.Foundation.Scripts
{
    internal abstract class EditorPrefsProperty<T>
    {
        private readonly T _defaultValue;
        private readonly Func<string, T, T> _getter;

        private readonly string _key;
        private readonly Action<string, T> _setter;

        private bool _isLoaded;
        private T _value;

        public EditorPrefsProperty(string key, T defaultValue, Func<string, T, T> getter, Action<string, T> setter)
        {
            _key = key;
            _defaultValue = defaultValue;
            _getter = getter;
            _setter = setter;
        }

        public T Value
        {
            get
            {
                if (!_isLoaded)
                {
                    _value = _getter(_key, _defaultValue);
                    _isLoaded = true;
                }

                return _value;
            }
            set
            {
                if (_isLoaded && _value.Equals(value))
                {
                    return;
                }

                _value = value;
                _setter(_key, value);
            }
        }
    }

    internal sealed class IntEditorPrefsProperty : EditorPrefsProperty<int>
    {
        public IntEditorPrefsProperty(string key, int defaultValue) : base(key, defaultValue, EditorPrefs.GetInt,
            EditorPrefs.SetInt)
        {
        }
    }

    internal sealed class FloatEditorPrefsProperty : EditorPrefsProperty<float>
    {
        public FloatEditorPrefsProperty(string key, float defaultValue) : base(key, defaultValue, EditorPrefs.GetFloat,
            EditorPrefs.SetFloat)
        {
        }
    }

    internal sealed class BoolEditorPrefsProperty : EditorPrefsProperty<bool>
    {
        public BoolEditorPrefsProperty(string key, bool defaultValue) : base(key, defaultValue, EditorPrefs.GetBool,
            EditorPrefs.SetBool)
        {
        }
    }

    internal sealed class StringEditorPrefsProperty : EditorPrefsProperty<string>
    {
        public StringEditorPrefsProperty(string key, string defaultValue) : base(key, defaultValue,
            EditorPrefs.GetString, EditorPrefs.SetString)
        {
        }
    }
}