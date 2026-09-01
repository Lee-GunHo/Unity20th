using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [Header("루트 패널 (SHOP 전체 패널)")]
    [SerializeField] private GameObject root;

    [System.Serializable]
    public class ShopItemEntry
    {
        [Header("아이템 정보")]
        public string label;                // 표시용 이름 (비워두면 ItemType 이름 사용)
        public ItemType itemType;
        public int price = 1;
        public Sprite sprite;

        [Header("UI 참조")]
        public Image itemImage;
        public TMP_Text nameText;
        public TMP_Text priceText;
        public Button buyButton;
    }

    [Header("상점 목록")]
    [SerializeField] private ShopItemEntry[] items;

    private PlayerWallet wallet;
    private PlayerInventory inventory;
    private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (root == null)
            root = gameObject;

        Debug.Log("[ShopUI] Awake 호출, 슬롯 세팅 시작");
        SetupSlots();
        Close();    // 시작할 땐 꺼두기
    }

    /// <summary>
    /// 각 아이템 슬롯 UI 세팅 & Buy 버튼 이벤트 연결
    /// </summary>
    private void SetupSlots()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("[ShopUI] items 배열이 비어 있습니다.");
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            var entry = items[i];
            if (entry == null)
            {
                Debug.LogWarning($"[ShopUI] {i}번 엔트리가 null 입니다.");
                continue;
            }

            // 아이콘
            if (entry.itemImage != null && entry.sprite != null)
                entry.itemImage.sprite = entry.sprite;

            // 이름
            if (entry.nameText != null)
            {
                if (!string.IsNullOrEmpty(entry.label))
                    entry.nameText.text = entry.label;
                else
                    entry.nameText.text = entry.itemType.ToString();
            }

            // 가격
            if (entry.priceText != null)
                entry.priceText.text = entry.price.ToString();

            // Buy 버튼 onClick 연결
            if (entry.buyButton != null)
            {
                int index = i; // 클로저용 로컬 변수
                entry.buyButton.onClick.RemoveAllListeners();
                entry.buyButton.onClick.AddListener(() => Buy(index));
                Debug.Log($"[ShopUI] {index}번 아이템 Buy 버튼 연결 완료 (버튼: {entry.buyButton.name})");
            }
            else
            {
                Debug.LogWarning($"[ShopUI] {i}번 아이템의 Buy Button이 비어 있습니다.");
            }
        }
    }

    /// <summary>
    /// 상점 열기 (NPC에서 호출)
    /// </summary>
    public void Open(PlayerWallet wallet, PlayerInventory inventory)
    {
        this.wallet = wallet;
        this.inventory = inventory;

        if (root != null)
            root.SetActive(true);

        isOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[ShopUI] 상점 열림");
    }

    /// <summary>
    /// 상점 닫기
    /// </summary>
    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        isOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("[ShopUI] 상점 닫힘");
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    /// <summary>
    /// Buy 버튼 클릭 처리
    /// </summary>
    private void Buy(int index)
    {
        Debug.Log($"[ShopUI] Buy 버튼 클릭 index={index}");

        if (!isOpen)
        {
            Debug.Log("[ShopUI] 상점이 열려있지 않습니다.");
            return;
        }

        if (wallet == null || inventory == null)
        {
            Debug.LogWarning("[ShopUI] wallet/inventory 미설정");
            return;
        }

        if (index < 0 || index >= items.Length)
        {
            Debug.LogWarning("[ShopUI] 잘못된 index");
            return;
        }

        var entry = items[index];
        if (entry == null)
        {
            Debug.LogWarning("[ShopUI] 빈 엔트리");
            return;
        }

        if (!wallet.Spend(entry.price))
        {
            Debug.Log("[ShopUI] 돈이 부족합니다.");
            return;
        }

        inventory.AddItem(entry.itemType);
        Debug.Log($"[ShopUI] {entry.itemType} 구매 완료!");
    }

    public void ApplyDiscountForMorning()
    {
        foreach (var entry in items)
        {
            if (entry.itemType == ItemType.Tuna)
            {
                entry.price = Mathf.RoundToInt(2000);
                if (entry.priceText != null)
                    entry.priceText.text = entry.price.ToString();

                Debug.Log("[ShopUI] 아침시간: 참치캔 할인 적용!");
            }
        }
    }

    public void ResetPrices()
    {
        foreach (var entry in items)
        {
            entry.price = Mathf.Max(1, entry.price);
            if (entry.priceText != null)
                entry.priceText.text = entry.price.ToString();
        }
    }

}
