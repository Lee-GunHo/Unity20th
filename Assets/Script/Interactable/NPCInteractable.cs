using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName;
    [TextArea][SerializeField] private string[] dialogueLines;

    public string GetPromptText()
    {
        return $"{npcName}에게 말 걸기 (F)";
    }

    public void Interact(PlayerInteraction player)
    {
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(npcName, dialogueLines);
        }
        else
        {
            Debug.LogWarning("[NPCInteractable] DialogueUI 인스턴스가 없습니다!");
        }
    }
}
