using System;
using System.IO;
using UnityEngine;

namespace Fabi.StickyNote
{
    public static class StickyNoteStore
    {
        static readonly string FilePath = Path.Combine("UserSettings", "StickyNote.txt");

        static string _text;

        public static event Action<string> Changed;

        public static string Text
        {
            get
            {
                _text ??= Load();
                return _text;
            }
            set
            {
                value ??= string.Empty;
                if (value == Text)
                    return;

                _text = value;
                Save();
                Changed?.Invoke(_text);
            }
        }

        static string Load()
        {
            try
            {
                return File.Exists(FilePath) ? File.ReadAllText(FilePath) : string.Empty;
            }
            catch (IOException e)
            {
                Debug.LogWarning($"Sticky Note: couldn't read {FilePath}: {e.Message}");
                return string.Empty;
            }
        }

        static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, _text);
            }
            catch (IOException e)
            {
                Debug.LogWarning($"Sticky Note: couldn't save {FilePath}: {e.Message}");
            }
        }
    }
}
