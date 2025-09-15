using UnityEngine;
using UnityEditor;
using Muco;

public class InteractibleMenuItems : MonoBehaviour
{
    private const string PrefabPath = "Packages/com.phenomenalviborg.muco/Runtime/Interactibles/Prefab/Button.prefab";
    [MenuItem("GameObject/MUCO/Button")]
    public static void SpawnMyPrefab()
    {
        // Load the prefab as a GameObject (works for both regular and variant prefabs)
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Could not find prefab at '{PrefabPath}'. Check the path.");
            return;
        }

        // Record the operation for undo support
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        // Instantiate the prefab into the active scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance != null)
        {
            // Optionally place it at the scene view camera position
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
                instance.transform.position = sceneView.pivot;

            // Register the new object with the undo system
            Undo.RegisterCreatedObjectUndo(instance, "Spawn MyPrefab");
            Selection.activeObject = instance;
        }
        else
        {
            Debug.LogError("Failed to instantiate prefab.");
        }

        Undo.CollapseUndoOperations(group);
    }

    // Optional: disable the menu item when the prefab can't be found
    [MenuItem("GameObject/MyPrefab", true)]
    private static bool ValidateSpawnMyPrefab()
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null;
    }
}
