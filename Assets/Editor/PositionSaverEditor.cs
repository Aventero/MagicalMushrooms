using System.Collections;
using System.Globalization;
using Unity.EditorCoroutines;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))] // This custom editor works with Transform components
public class PositionSaverEditor : Editor
{
    private GameObject selectedObject; // Store the selected object
    private bool autoRestoreEnabled = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Check if a GameObject is selected
        if (Selection.activeGameObject != null)
        {
            selectedObject = Selection.activeGameObject;


            EditorGUILayout.Space();

            // Save Button
            if (GUILayout.Button("Save Transform"))
            {
                // Save the position of the selected object
                SaveTransform(selectedObject);
            }

            // Restore Button
            if (GUILayout.Button("Restore"))
            {
                // Restore the position of the selected object
                RestoreTransform(selectedObject);
            }

            // Enable/Disable Auto-Restore Toggle
            autoRestoreEnabled = GUILayout.Toggle(autoRestoreEnabled, "Auto-Restore");
        }
    }

    private void SaveTransform(GameObject obj)
    {
        // Check if the object has a Transform component
        if (obj.TryGetComponent<Transform>(out Transform transform))
        {
            // Store the object's current position
            EditorPrefs.SetString($"{obj.name}_Position", transform.position.ToString());
            EditorPrefs.SetString($"{obj.name}_Rotation", transform.rotation.eulerAngles.ToString());
            Debug.Log($"Saved transform");
        }
    }

    private void RestoreTransform(GameObject obj)
    {

        // Check if the object has a Transform component
        if (obj.TryGetComponent<Transform>(out Transform transform))
        {
            // Try to retrieve the saved position
            if (EditorPrefs.HasKey($"{obj.name}_Position"))
            {
                Vector3 savedPosition = StringToVector3(EditorPrefs.GetString($"{obj.name}_Position"));
                Vector3 savedRotation = StringToVector3(EditorPrefs.GetString($"{obj.name}_Rotation"));

                transform.position = savedPosition;
                transform.rotation = Quaternion.Euler(savedRotation);
                Debug.Log($"Restored transform: ");

                EditorPrefs.DeleteKey($"{selectedObject.name}_Position");
                EditorPrefs.DeleteKey($"{selectedObject.name}_Rotation");
            }
        }
    }

    // Helper method to convert a string to a Vector3
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0], NumberStyles.Any, CultureInfo.InvariantCulture),
            float.Parse(sArray[1], NumberStyles.Any, CultureInfo.InvariantCulture),
            float.Parse(sArray[2], NumberStyles.Any, CultureInfo.InvariantCulture));

        return result;
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode && autoRestoreEnabled && selectedObject != null)
        {
            // Automatically restore the position and rotation when exiting play mode
            EditorCoroutineUtility.StartCoroutineOwnerless(RestoreTransformDelayed());
        }
    }

    IEnumerator RestoreTransformDelayed()
    {
        yield return new WaitForSeconds(0.01f);
        RestoreTransform(selectedObject);
    }
}