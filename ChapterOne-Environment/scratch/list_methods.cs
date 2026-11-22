var t = System.Type.GetType("ThatNight.ChapterOne.Editor.StiltHouseBuilder, ThatNight.ChapterOne.Editor");
if (t == null) {
    foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
        t = asm.GetType("ThatNight.ChapterOne.Editor.StiltHouseBuilder");
        if (t != null) break;
    }
}
var sb = new System.Text.StringBuilder();
sb.AppendLine("Type: " + (t != null ? t.FullName : "null"));
if (t != null) {
    foreach (var m in t.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)) {
        sb.AppendLine("- " + m.Name);
    }
}
return sb.ToString();
