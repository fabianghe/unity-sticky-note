using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace Fabi.StickyNote
{
    [Overlay(typeof(SceneView), Id, "Sticky Note", true,
        defaultDockZone = DockZone.RightColumn, defaultLayout = Layout.Panel,
        defaultWidth = 240f, defaultHeight = 150f, minWidth = 160f, minHeight = 90f, maxWidth = 600f, maxHeight = 600f)]
    public class StickyNoteOverlay : Overlay
    {
        public const string Id = "fabi-sticky-note";

        public StickyNoteOverlay() => collapsedIcon = StickyNoteIcon.Get();

        public override VisualElement CreatePanelContent() => new StickyNoteView();
    }
}
