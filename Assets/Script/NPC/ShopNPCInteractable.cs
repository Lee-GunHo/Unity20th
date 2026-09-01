using UnityEngine;

public class ShopNPCInteractable : MonoBehaviour, IInteractable
{
    [Header("NPC Info")]
    [SerializeField] private string npcName = "상점 주인";
    [TextArea]
    [SerializeField] private string[] dialogueLines;

    [Header("References")]
    [SerializeField] private ShopUI shopUI;    // 씬의 ShopUI 오브젝트 드래그

    // F키 안내 문구 등
    public string GetPromptText()
    {
        return $"{npcName}에게 말 걸기 (F)";
    }

    // PlayerInteraction에서 호출
    public void Interact(PlayerInteraction player)
    {
        if (player == null)
        {
            Debug.LogWarning("[ShopNPC] PlayerInteraction이 없습니다.");
            return;
        }

        if (shopUI == null)
        {
            Debug.LogWarning("[ShopNPC] ShopUI가 연결되지 않았습니다.");
            return;
        }

        var wallet = player.GetComponent<PlayerWallet>();
        var inventory = player.GetComponent<PlayerInventory>();

        if (wallet == null)
        {
            Debug.LogWarning("[ShopNPC] PlayerWallet 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        if (inventory == null)
        {
            Debug.LogWarning("[ShopNPC] PlayerInventory 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        if (DialogueUI.Instance == null)
        {
            Debug.LogWarning("[ShopNPC] DialogueUI 인스턴스가 없습니다.");
            return;
        }

        Debug.Log("[ShopNPC] 상점 대화 시작");

        // 1) 대화 UI(comnu) 켜고
        // 2) 대화가 끝나면(DialogueUI.EndDialogue) 자동으로 comnu를 끈 뒤
        // 3) 콜백 안에서 상점 열기
        DialogueUI.Instance.StartDialogue(npcName, dialogueLines, () =>
        {
            Debug.Log("[ShopNPC] 대화 끝 → 상점 오픈");
            shopUI.Open(wallet, inventory);
        });
    }
}
