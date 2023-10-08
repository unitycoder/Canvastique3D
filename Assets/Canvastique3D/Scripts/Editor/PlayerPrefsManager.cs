using UnityEngine;
using UnityEditor;

public class PlayerPrefsManager
{
    [MenuItem("Tools/PlayerPrefs/Delete All")]
    private static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted!");
    }

    [MenuItem("Tools/PlayerPrefs/Delete Specific Key")]
    private static void DeleteSpecificKey()
    {
        string key = "YourKeyName";  // Change this to your key
        PlayerPrefs.DeleteKey(key);
        Debug.Log($"PlayerPrefs key '{key}' deleted!");
    }
}
