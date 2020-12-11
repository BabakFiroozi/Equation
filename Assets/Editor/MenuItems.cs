using System;
using UnityEngine;
using UnityEditor;


namespace Equation.Tools
{
    public class MenuItems
    {

        [MenuItem("Assets/Create/Game Config")]
        public static void CreateMyAsset()
        {
            var asset = ScriptableObject.CreateInstance<GameConfig>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/GameConfig.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        
        
        
        //	[MenuItem("Tools/Config Guilds")]
        //	public static void ConfigGuilds()
        //	{
        //		Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/"+ "GuildFactory" +".prefab");
        //	}

        [MenuItem("Tools/Clear Player Prefs")]
        public static void ClearPref()
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure to clear player prefrences?", "Yes", "No"))
                PlayerPrefs.DeleteAll();
        }
        
        
    }
}