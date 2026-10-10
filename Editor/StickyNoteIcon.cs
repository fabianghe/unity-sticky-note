using UnityEngine;

namespace Fabi.StickyNote
{
    static class StickyNoteIcon
    {
        const int Size = 16;

        static Texture2D _icon;

        public static Texture2D Get()
        {
            if (_icon != null)
                return _icon;

            _icon = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Point
            };

            var pixels = new Color[Size * Size];
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    bool inside = x >= 2 && x <= 13 && y >= 2 && y <= 13;
                    bool foldedCorner = x + (Size - 1 - y) > 22;
                    bool strip = y >= 11;
                    Color color = strip ? new Color(0.96f, 0.84f, 0.3f) : StickyNoteView.Paper;
                    pixels[y * Size + x] = inside && !foldedCorner ? color : Color.clear;
                }
            }

            _icon.SetPixels(pixels);
            _icon.Apply();
            return _icon;
        }
    }
}
