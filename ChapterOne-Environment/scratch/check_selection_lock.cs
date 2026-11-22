var sb = new System.Text.StringBuilder();

sb.AppendLine("isCompiling: " + UnityEditor.EditorApplication.isCompiling);
sb.AppendLine("isUpdating: " + UnityEditor.EditorApplication.isUpdating);
sb.AppendLine("isPlaying: " + UnityEditor.EditorApplication.isPlaying);

var sel = UnityEditor.Selection.activeGameObject;
if (sel == null) {
    sb.AppendLine("Selection: NONE (nothing selected in Unity)");
} else {
    sb.AppendLine("Selected GameObject: " + sel.name);
    sb.AppendLine("isStatic: " + sel.isStatic);
    sb.AppendLine("hideFlags: " + sel.hideFlags);
    sb.AppendLine("transform.position: " + sel.transform.position);
    sb.AppendLine("parent: " + (sel.transform.parent != null ? sel.transform.parent.name : "root"));
    sb.AppendLine("childCount: " + sel.transform.childCount);
    sb.AppendLine("activeTool: " + UnityEditor.Tools.current);
    sb.AppendLine("pivotMode: " + UnityEditor.Tools.pivotMode);
    sb.AppendLine("pivotRotation: " + UnityEditor.Tools.pivotRotation);
}

return sb.ToString();
