---
uid: rich_text_reference
---
# Rich Text Reference
This page contains a rich text reference for broadcasts and hints that can be used in EXILED plugins.

## Rendering Tags
Applies bold or italics to the broadcast or hint.
- `<b>Text</b>` would produce <b>Text</b>.
- `<i>Text</i>` would produce <i>Text</i>.

## Color
Applies a color to the broadcast or hint.
- `<color=#FF0000>Text</color>` would produce <span style="color:#FF0000;">Text</span>.
- `<color=green>Text</color>` would produce <span style="color:#008000;">Text</span>.


See [this table](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html#ColorNames) for a list of supported color names.

## Size
Changes the size of the broadcast or hint.
- `<size=30>Text</size>` would increase the size of the text.
- `<size=5>Text</size>` would reduce the size of the text.
