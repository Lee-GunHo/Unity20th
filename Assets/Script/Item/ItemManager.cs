using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [System.Serializable]
    public class ItemData
    {
        public ItemType type;          // 아이템 타입
        public string displayName;     // UI 표기 이름
        public GameObject handPrefab;  // 손에 들 프리팹 (IToolUsable 포함)
        public GameObject dropPrefab;  // 바닥에 둘 프리팹
        public Sprite icon;
    }

    [Header("등록된 아이템들")]
    public ItemData[] items;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public ItemData Get(ItemType type)
    {
        foreach (var item in items)
        {
            if (item.type == type)
                return item;
        }

        Debug.LogError($"[ItemManager] {type} 데이터가 없습니다.");
        return null;
    }
}
