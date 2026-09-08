using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Utilidad de migración: reemplaza en bloque los componentes SpatialTriggerEvent
/// (ahora "Missing Script" porque el SDK de Spatial ya no está en el proyecto) por
/// PlayerTriggerZone en todo objeto de la escena que tenga un Collider en modo Trigger.
/// No reconecta los UnityEvents (esa información se perdió junto con el script original);
/// eso hay que volver a cablearlo a mano en el Inspector tras ejecutar esto.
/// </summary>
public static class FixBrokenSpatialTriggers
{
    [MenuItem("Tools/Migración Spatial/Reemplazar triggers rotos por PlayerTriggerZone")]
    public static void ReplaceBrokenTriggers()
    {
        int fixedCount = 0;
        int skippedCount = 0;

        foreach (GameObject go in Object.FindObjectsOfType<GameObject>(true))
        {
            if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go) == 0)
                continue;

            Collider col = go.GetComponent<Collider>();
            if (col == null || !col.isTrigger)
            {
                skippedCount++;
                Debug.LogWarning($"[FixBrokenSpatialTriggers] '{go.name}' tiene un script roto pero no un Collider en modo Trigger; se dejó sin tocar.", go);
                continue;
            }

            if (go.GetComponent<PlayerTriggerZone>() == null)
            {
                Undo.AddComponent<PlayerTriggerZone>(go);
                fixedCount++;
            }

            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            EditorUtility.SetDirty(go);
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log($"[FixBrokenSpatialTriggers] PlayerTriggerZone añadido a {fixedCount} objeto(s). " +
                  $"{skippedCount} objeto(s) con script roto no tenían Collider trigger y necesitan revisión manual. " +
                  "Recuerda: hay que volver a asignar los UnityEvents (On Player Enter / On Player Exit) de cada uno en el Inspector.");
    }
}
