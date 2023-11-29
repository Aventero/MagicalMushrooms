using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DialogManager : MonoBehaviour
{
    [SerializeField] // This makes it private in code but visible in the Inspector.
    private List<Dialog> dialogs;

    private readonly Dictionary<string, Dialog> nameToDialog = new();

    public static DialogManager Instance { get; private set; }
    public PlayableDirector PlayableDirector;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            PopulateNameToDialogDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PlayableDirector == null)
            return;

        StateManager.Instance.StartedDialogEvent.AddListener(() => PlayableDirector.Pause());
        StateManager.Instance.EndedDialogEvent.AddListener(() => PlayableDirector.Resume());
    }

    private void PopulateNameToDialogDictionary()
    {
        foreach (var dialog in dialogs)
        {
            if (nameToDialog.ContainsKey(dialog.name))
            {
                Debug.LogError("Multiple dialogs with the name: " + dialog.name);
                continue;
            }
            nameToDialog.Add(dialog.name, dialog);
            Debug.Log("Added: " + dialog.name);
        }
    }

    public void PlayDialog(string name)
    {
        if (nameToDialog.TryGetValue(name, out Dialog dialog))
        {
            UIManager.Instance.ShowDialog(dialog);
        }
        else
        {
            Debug.LogWarning($"Dialog with name {name} not found!");
        }
    }


}
