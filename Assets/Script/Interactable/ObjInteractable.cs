using UnityEngine;

public class ObjInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string ObjName;
    [TextArea][SerializeField] private string[] dialogueLines;

    public string GetPromptText()
    {
        return $"{ObjName}에서 상호작용 하기 (F)";
    }

    public void Interact(PlayerInteraction player)
    {
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(ObjName, dialogueLines);
        }
        else
        {
            Debug.LogWarning("[NPCInteractable] DialogueUI �ν��Ͻ��� �����ϴ�!");
        }
    }
}