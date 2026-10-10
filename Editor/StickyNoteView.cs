using UnityEngine;
using UnityEngine.UIElements;

namespace Fabi.StickyNote
{
    public class StickyNoteView : VisualElement
    {
        public static readonly Color Paper = new(1f, 0.92f, 0.45f);
        static readonly Color Strip = new(0.96f, 0.84f, 0.3f);
        static readonly Color Ink = new(0.2f, 0.16f, 0.02f);
        static readonly Color Hint = new(0.2f, 0.16f, 0.02f, 0.45f);
        const string Placeholder = "What are you doing right now?";
        const float FontSize = 14f;

        static readonly (string Name, string Hex)[] InkColors =
        {
            ("Green", "#1E7B34"),
            ("Red", "#C62828"),
            ("Blue", "#1565C0"),
            ("Purple", "#6A1B9A"),
        };

        readonly TextField _field;
        readonly ScrollView _preview;
        readonly Label _rendered;
        int _selectionStart;
        int _selectionEnd;

        public StickyNoteView()
        {
            style.flexGrow = 1;
            style.minWidth = 160;
            style.minHeight = 90;
            style.backgroundColor = Paper;
            style.borderTopLeftRadius = style.borderTopRightRadius = style.borderBottomLeftRadius = style.borderBottomRightRadius = 3;
            style.overflow = Overflow.Hidden;

            Add(CreateToolbar());

            _field = new TextField { multiline = true, value = StickyNoteStore.Text, selectAllOnFocus = false, selectAllOnMouseUp = false };
            _field.textEdition.placeholder = Placeholder;
            _field.verticalScrollerVisibility = ScrollerVisibility.Auto;
            _field.style.flexGrow = 1;
            _field.style.marginLeft = _field.style.marginRight = _field.style.marginTop = _field.style.marginBottom = 0;
            _field.RegisterValueChangedCallback(evt =>
            {
                StickyNoteStore.Text = evt.newValue;
                Refresh();
            });
            _field.RegisterCallback<KeyUpEvent>(_ => CacheSelection(), TrickleDown.TrickleDown);
            _field.RegisterCallback<PointerUpEvent>(_ => CacheSelection(), TrickleDown.TrickleDown);
            _field.RegisterCallback<FocusOutEvent>(_ =>
            {
                CacheSelection();
                ShowRendered(!string.IsNullOrEmpty(_field.value));
            });
            Add(_field);

            var input = _field.Q(className: TextField.inputUssClassName);
            input.style.backgroundColor = Color.clear;
            input.style.borderTopWidth = input.style.borderBottomWidth = input.style.borderLeftWidth = input.style.borderRightWidth = 0;
            Pad(input);
            input.style.color = Ink;
            input.style.fontSize = FontSize;
            input.style.whiteSpace = WhiteSpace.Normal;
            input.style.flexGrow = 1;
            input.style.alignItems = Align.FlexStart;

            var text = input.Q<TextElement>();
            if (text != null)
            {
                text.style.whiteSpace = WhiteSpace.Normal;
                text.enableRichText = false;
            }

            _rendered = new Label { enableRichText = true };
            _rendered.style.whiteSpace = WhiteSpace.Normal;
            _rendered.style.color = Ink;
            _rendered.style.fontSize = FontSize;
            _rendered.style.marginLeft = _rendered.style.marginRight = _rendered.style.marginTop = _rendered.style.marginBottom = 0;
            Pad(_rendered);

            _preview = new ScrollView(ScrollViewMode.Vertical);
            _preview.style.flexGrow = 1;
            _preview.Add(_rendered);
            _preview.RegisterCallback<PointerDownEvent>(evt =>
            {
                evt.StopPropagation();
                StartEditing(_field.value.Length);
            }, TrickleDown.TrickleDown);
            Add(_preview);

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                StickyNoteStore.Changed += HandleChanged;
                HandleChanged(StickyNoteStore.Text);
            });
            RegisterCallback<DetachFromPanelEvent>(_ => StickyNoteStore.Changed -= HandleChanged);
            RegisterCallback<GeometryChangedEvent>(_ => TintPlaceholder());

            _selectionStart = _selectionEnd = _field.value.Length;
            Refresh();
            ShowRendered(!string.IsNullOrEmpty(_field.value));
        }

        VisualElement CreateToolbar()
        {
            var toolbar = new VisualElement();
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.alignItems = Align.Center;
            toolbar.style.height = 20;
            toolbar.style.paddingLeft = 4;
            toolbar.style.backgroundColor = Strip;

            var bold = CreateToolButton("Bold", () => Wrap("<b>", "</b>"));
            bold.text = "B";
            bold.style.unityFontStyleAndWeight = FontStyle.Bold;
            bold.style.color = Ink;
            bold.style.fontSize = 12;
            toolbar.Add(bold);

            foreach (var (name, hex) in InkColors)
            {
                ColorUtility.TryParseHtmlString(hex, out Color color);
                var swatch = CreateToolButton(name, () => Wrap($"<color={hex}>", "</color>"));
                swatch.style.backgroundColor = color;
                swatch.style.width = swatch.style.height = 12;
                swatch.style.borderTopLeftRadius = swatch.style.borderTopRightRadius = swatch.style.borderBottomLeftRadius = swatch.style.borderBottomRightRadius = 6;
                toolbar.Add(swatch);
            }

            return toolbar;
        }

        static Button CreateToolButton(string tooltip, System.Action clicked)
        {
            var button = new Button(clicked) { tooltip = tooltip, focusable = false };
            button.style.width = 18;
            button.style.height = 16;
            button.style.marginLeft = button.style.marginRight = 2;
            button.style.marginTop = button.style.marginBottom = 0;
            button.style.paddingLeft = button.style.paddingRight = button.style.paddingTop = button.style.paddingBottom = 0;
            button.style.borderTopWidth = button.style.borderBottomWidth = button.style.borderLeftWidth = button.style.borderRightWidth = 0;
            button.style.backgroundColor = Color.clear;
            return button;
        }

        static void Pad(VisualElement element)
        {
            element.style.paddingLeft = element.style.paddingRight = 8;
            element.style.paddingTop = element.style.paddingBottom = 6;
        }

        void CacheSelection()
        {
            _selectionStart = Mathf.Min(_field.cursorIndex, _field.selectIndex);
            _selectionEnd = Mathf.Max(_field.cursorIndex, _field.selectIndex);
        }

        void Wrap(string open, string close)
        {
            string text = _field.value;
            int start = Mathf.Clamp(_selectionStart, 0, text.Length);
            int end = Mathf.Clamp(_selectionEnd, start, text.Length);

            _field.value = text.Substring(0, start) + open + text.Substring(start, end - start) + close + text.Substring(end);

            if (start == end)
            {
                StartEditing(start + open.Length);
                return;
            }

            _selectionStart = _selectionEnd = end + open.Length + close.Length;
            ShowRendered(true);
        }

        void StartEditing(int cursor)
        {
            ShowRendered(false);
            _field.schedule.Execute(() =>
            {
                _field.Focus();
                _field.SelectRange(cursor, cursor);
                CacheSelection();
            });
        }

        void ShowRendered(bool rendered)
        {
            _field.style.display = rendered ? DisplayStyle.None : DisplayStyle.Flex;
            _preview.style.display = rendered ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void HandleChanged(string text)
        {
            if (_field.value == text)
                return;

            _field.SetValueWithoutNotify(text);
            Refresh();
        }

        void Refresh()
        {
            _rendered.text = _field.value;
            TintPlaceholder();
        }

        void TintPlaceholder()
        {
            var text = _field.Q<TextElement>();
            if (text != null)
                text.style.color = string.IsNullOrEmpty(_field.value) ? Hint : Ink;
        }
    }
}
