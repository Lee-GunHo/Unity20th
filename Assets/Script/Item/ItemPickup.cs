using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemType itemType;
    [Header("SFX")]
    public AudioClip AudioGetItem;
    [Range(0f, 1f)] public float sfxvolume = 1f;

    public string GetPromptText()
    {
        if (ItemManager.Instance != null)
        {
            var data = ItemManager.Instance.Get(itemType);
            if (data != null && !string.IsNullOrWhiteSpace(data.displayName))
                return $"F Ű: {data.displayName} �ݱ�";
        }

        return $"F Ű: {itemType} �ݱ�";
    }

    public void Interact(PlayerInteraction player)
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogWarning("[ItemPickup] PlayerInventory ����");
            return;
        }

        PlayerInventory.Instance.AddItem(itemType);
        Debug.Log($"[ItemPickup] {itemType} ȹ��");

        if (AudioGetItem != null)
            AudioSource.PlayClipAtPoint(AudioGetItem, transform.position, sfxvolume);
        Destroy(gameObject);
    }
}
