using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

public class AddMenu : EditorWindow {
    [MenuItem("Edit/Reset Playerprefs")]
    public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }
}

#endif
    