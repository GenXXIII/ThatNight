ThatNight.ChapterOne.Editor.SurfaceLibrary.Prepare();
var pink = ThatNight.ChapterOne.Editor.SurfaceLibrary.Pink;
var gold = ThatNight.ChapterOne.Editor.SurfaceLibrary.Plain("Carpet gold piping", new UnityEngine.Color(.82f, .65f, .18f), .45f, .25f);
return $"Pink mat: name={pink?.name}, shader={pink?.shader?.name}, color={pink?.color}; Gold mat: name={gold?.name}, shader={gold?.shader?.name}";
