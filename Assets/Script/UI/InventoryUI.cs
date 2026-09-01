// InventoryUI.cs

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Root")]
    [SerializeField] private GameObject root;

    [System.Serializable]
    public class SlotUI
    {
        public Image icon;               // 아이템 이미지
        public TextMeshProUGUI count;    // 개수 텍스트
        public GameObject effect;        // 선택 슬롯 표시 오브젝트
    }

    [Header("슬롯 UI")]
    [SerializeField] private SlotUI[] slots;

    [Header("Money")]
    [SerializeField] private TextMeshProUGUI moneyText;
    public Image moneyIcon;               // 아이템 이미지

    private PlayerInventory inventory;
    private PlayerWallet wallet;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 항상 켜둘 UI이므로 시작부터 활성화
        if (root != null)
            root.SetActive(true);
    }

    private void Start()
    {
        inventory = PlayerInventory.Instance;
        wallet = FindAnyObjectByType<PlayerWallet>();

        RefreshAll();
        RefreshMoney();
    }

    private void Update()
    {
        RefreshMoney(); // 돈 자주 바뀌지 않으면 이벤트로 빼도 되지만, 일단 간단하게
    }

    public void RefreshAll()
    {
        if (inventory == null || slots == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            var ui = slots[i];
            var info = inventory.GetSlotInfo(i + 1);

            if (!info.hasItem)
            {
                if (ui.icon != null)
                {
                    ui.icon.enabled = false;
                    ui.icon.sprite = null;
                }
                if (ui.count != null)
                    ui.count.text = "";
            }
            else
            {
                if (ui.icon != null)
                {
                    ui.icon.enabled = true;
                    if (info.icon != null)
                        ui.icon.sprite = info.icon;
                }

                if (ui.count != null)
                    ui.count.text = (info.count > 1) ? info.count.ToString() : "";
            }

            if (ui.effect != null)
                ui.effect.SetActive(info.isSelected);
        }
    }

    public void OnClickSlot(int index)
    {
        // index는 0부터라고 가정하고, 버튼 OnClick에서 넣어주기
        if (inventory == null) return;

        inventory.EquipSlotFromUI(index + 1);
        RefreshAll();
    }

    private void RefreshMoney()
    {
        if (moneyText == null || wallet == null) return;
        moneyText.text = wallet.Money.ToString();
    }
}
