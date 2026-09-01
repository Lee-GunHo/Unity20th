using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 인벤토리 관리:
/// - 슬롯 4개
/// - 아이템 추가/삭제/선택
/// - InventoryUI와 PlayerToolController 연동
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("참조")]
    [SerializeField] private PlayerToolController toolController;
    [SerializeField] private Transform dropPoint;

    private GameObject[] heldObjects = new GameObject[4];
    private GameObject[] dropPrefabs = new GameObject[4];
    private string[] itemNames = new string[4];
    private ItemType[] itemTypes = new ItemType[4];
    private int[] itemCounts = new int[4];

    private int currentSlot = 0; // 1~4 중 선택 슬롯 (0 = 없음)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HideAllItems();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.digit1Key.wasPressedThisFrame) EquipSlot(1);
        if (kb.digit2Key.wasPressedThisFrame) EquipSlot(2);
        if (kb.digit3Key.wasPressedThisFrame) EquipSlot(3);
        if (kb.digit4Key.wasPressedThisFrame) EquipSlot(4);

        if (kb.gKey.wasPressedThisFrame) DropCurrentItem();
    }

    // ─────────────────────────────
    // 슬롯 전환
    // ─────────────────────────────
    private void EquipSlot(int slot)
    {
        if (slot < 1 || slot > 4) return;
        if (currentSlot == slot) return;

        int index = slot - 1;

        if (heldObjects[index] == null)
        {
            toolController?.ClearTool();
            currentSlot = 0;
            NotifyChanged();
            return;
        }

        currentSlot = slot;
        HideAllItems();

        var obj = heldObjects[index];
        obj.SetActive(true);

        if (toolController != null)
        {
            var tool = obj.GetComponent<IToolUsable>();
            if (tool != null)
                toolController.EquipExistingTool(tool);
            else
                toolController.ClearTool();
        }

        NotifyChanged();
    }

    private void HideAllItems()
    {
        for (int i = 0; i < heldObjects.Length; i++)
        {
            if (heldObjects[i] != null)
                heldObjects[i].SetActive(false);
        }
    }

    // ─────────────────────────────
    // 아이템 드롭
    // ─────────────────────────────
    private void DropCurrentItem()
    {
        if (currentSlot <= 0 || currentSlot > 4) return;

        int index = currentSlot - 1;
        if (heldObjects[index] == null) return;

        // 개수 줄이기
        if (itemCounts[index] > 1)
        {
            itemCounts[index]--;
            NotifyChanged();
            return;
        }

        string name = itemNames[index];
        var dropPrefab = dropPrefabs[index];
        var handObj = heldObjects[index];

        heldObjects[index] = null;
        itemNames[index] = null;
        dropPrefabs[index] = null;
        itemTypes[index] = default;
        itemCounts[index] = 0;

        toolController?.ClearTool();
        Destroy(handObj);

        SpawnDrop(dropPrefab);

        currentSlot = 0;
        HideAllItems();
        NotifyChanged();
    }

    private void SpawnDrop(GameObject prefab)
    {
        if (prefab == null || dropPoint == null) return;

        Vector3 pos = dropPoint.position + dropPoint.forward * 1.2f + Vector3.up * 0.3f;
        Instantiate(prefab, pos, Quaternion.identity);
    }

    // ─────────────────────────────
    // 아이템 추가
    // ─────────────────────────────
    public void AddItem(ItemType type)
    {
        if (ItemManager.Instance == null)
        {
            Debug.LogError("[Inventory] ItemManager 없음");
            return;
        }

        var data = ItemManager.Instance.Get(type);
        if (data == null)
        {
            Debug.LogError($"[Inventory] ItemData 없음: {type}");
            return;
        }

        AddItem(data.handPrefab, data.dropPrefab, data.displayName, type);
    }

    private void AddItem(GameObject handPrefab, GameObject dropPrefab, string name, ItemType type)
    {
        if (handPrefab == null)
        {
            Debug.LogError($"[Inventory] {name} handPrefab 없음");
            return;
        }

        // 이미 같은 타입 있으면 개수만 증가
        int exist = FindSlotWithType(type);
        if (exist != -1)
        {
            itemCounts[exist]++;
            NotifyChanged();
            return;
        }

        // 비어있는 슬롯 찾기
        int slot = FindFirstEmptySlot();
        if (slot == -1)
        {
            Debug.Log("[Inventory] 슬롯 가득 참");
            return;
        }

        // 손 프리팹 생성
        Transform parent = toolController != null ? toolController.HandSocket : transform;
        var instance = Instantiate(handPrefab, parent);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.SetActive(false);

        heldObjects[slot] = instance;
        dropPrefabs[slot] = dropPrefab;
        itemNames[slot] = name;
        itemTypes[slot] = type;
        itemCounts[slot] = 1;

        if (currentSlot == 0)
            EquipSlot(slot + 1);

        NotifyChanged();
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < heldObjects.Length; i++)
            if (heldObjects[i] == null)
                return i;
        return -1;
    }

    private int FindSlotWithType(ItemType type)
    {
        for (int i = 0; i < itemTypes.Length; i++)
        {
            if (heldObjects[i] != null && itemTypes[i] == type)
                return i;
        }
        return -1;
    }

    // ─────────────────────────────
    // UI용 데이터 구조
    // ─────────────────────────────
    public struct SlotInfo
    {
        public bool hasItem;
        public string itemName;
        public Sprite icon;
        public int count;
        public bool isSelected;
    }

    public SlotInfo GetSlotInfo(int slot)
    {
        SlotInfo info = new SlotInfo
        {
            hasItem = false,
            itemName = null,
            icon = null,
            count = 0,
            isSelected = (currentSlot == slot)
        };

        if (slot < 1 || slot > 4) return info;

        int index = slot - 1;
        if (heldObjects[index] != null)
        {
            info.hasItem = true;
            info.itemName = itemNames[index];
            info.count = Mathf.Max(1, itemCounts[index]);

            if (ItemManager.Instance != null)
            {
                var data = ItemManager.Instance.Get(itemTypes[index]);
                if (data != null)
                    info.icon = data.icon;
            }
        }

        return info;
    }

    public void EquipSlotFromUI(int slot)
    {
        EquipSlot(slot);
    }

    // ─────────────────────────────
    // UI 갱신 통보
    // ─────────────────────────────
    private void NotifyChanged()
    {
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.RefreshAll();
    }
}
