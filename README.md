# Sticky Note for Unity

A yellow sticky note that lives in the Unity Editor to keep the task at hand in front of you.

- Always visible as a **Scene view overlay** (drag, float, resize or collapse it; toggle it from the `` ` `` overlay menu).
- Also available as a dockable window: **Window → Sticky Note**.
- **Bold** and four colors (green, red, blue, purple) from the toolbar: select text while editing, then click **B** or a color. Click outside the note to see the formatted text.
- Saved per project in `UserSettings/StickyNote.txt` (ignored by Unity's standard `.gitignore`, so your note stays personal).

Requires Unity 6.

## Install

In your project's `Packages/manifest.json`, add to `dependencies`:

```json
"com.fabi.sticky-note": "https://github.com/fabianghe/unity-sticky-note.git"
```

Or in Unity: **Window → Package Manager → + → Install package from git URL…** and paste the URL.

To pin a version, append a tag: `https://github.com/fabianghe/unity-sticky-note.git#v1.0.0`.

## Working on it

Clone this repo, then point a project at your clone so edits show up live:

```json
"com.fabi.sticky-note": "file:E:/Repos/unity-sticky-note"
```

Switch back to the git URL when you're done, commit and push here, and other projects get the update the next time they refresh the package (remove its entry from `packages-lock.json` to force it).
