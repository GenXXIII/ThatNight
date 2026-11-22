using UnityEditor;

namespace ThatNight.ChapterOne.Editor
{
    /// <summary>
    /// Ensures MCP for Unity bridge auto-starts and reconnects on editor domain reloads.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoStartMcpSession
    {
        static AutoStartMcpSession()
        {
            EditorPrefs.SetBool("MCPForUnity.AutoStartOnLoad", true);
        }
    }
}
