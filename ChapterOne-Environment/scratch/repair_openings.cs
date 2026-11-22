using UnityEngine;
using System.Reflection;
using ThatNight.ChapterOne.Editor;
var asm=typeof(FirstPersonGameplayBuilder).Assembly;
var restore=asm.GetType("ThatNight.ChapterOne.Editor.OpeningMeshSplitter").GetMethod("Restore");
foreach(var f in GameObject.Find("01 HOUSE").GetComponentsInChildren<MeshFilter>())if(f&&f.GetComponentInChildren<ThatNight.ChapterOne.HingedOpening>()&&!f.GetComponent<ThatNight.ChapterOne.HingedOpening>())restore.Invoke(null,new object[]{f});
asm.GetType("ThatNight.ChapterOne.Editor.CharacterArtBuilder").GetMethod("Prepare").Invoke(null,null);
return asm.GetType("ThatNight.ChapterOne.Editor.GameplayOpeningBuilder").GetMethod("Apply").Invoke(null,null);
