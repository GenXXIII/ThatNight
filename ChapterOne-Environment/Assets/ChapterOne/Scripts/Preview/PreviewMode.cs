using System;

namespace ThatNight.ChapterOne
{
    public static class PreviewMode
    {
        public static bool EnvironmentPreview => Has("-environmentPreview");
        public static bool EnvironmentReview => Has("-environmentReview");
        public static bool ForestPreview => Has("-forestPreview");
        public static bool GameplayReview => Has("-gameplayReview");
        public static bool SkipStory => EnvironmentPreview || EnvironmentReview || ForestPreview || GameplayReview;
        static bool Has(string flag) => Array.IndexOf(Environment.GetCommandLineArgs(), flag) >= 0;
    }
}
