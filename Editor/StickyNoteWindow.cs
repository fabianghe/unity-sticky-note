using UnityEditor;
using UnityEngine;

namespace Fabi.StickyNote
{
    public class StickyNoteWindow : EditorWindow
    {
        [MenuItem("Window/Sticky Note")]
        static void Open()
        {
            var window = GetWindow<StickyNoteWindow>();
            window.minSize = new Vector2(160f, 90f);
            window.Show();
        }

        void OnEnable() => titleContent = new GUIContent("Sticky Note", StickyNoteIcon.Get());

        void CreateGUI() => rootVisualElement.Add(new StickyNoteView());
    }
}
