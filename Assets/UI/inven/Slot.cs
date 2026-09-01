//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using static UnityEditor.Progress;

//public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//{
//    [SerializeField] private Image slot_Item_Image;
//    [SerializeField] private Image Event_Image;
//    [SerializeField] private TMP_Text slot_Item_Count_TXT;

//    private Item item;       // ������ ������
//    private bool isIn = false;

//    private Canvas canvas;
//    private GameObject draggingIcon;
//    private RectTransform draggingPlane;

//    void Awake()
//    {
//        canvas = FindObjectOfType<Canvas>();
//        draggingPlane = canvas.transform as RectTransform;
//        slot_Item_Count_TXT.text = "";
//    }

//    public void SetSlot(Item setitem, bool is_New_Item)
//    {
//        if (setitem != null)
//        {
//            GetItem(setitem);
//            Slot_Update(setitem);
//        }
//    }

//    private void GetItem(Item item)
//    {
//        // item.SetIsGet(true);
//    }

//    private void Slot_Update(Item update_Item)
//    {
//        SetColor(1f, slot_Item_Image);
//        // this.slot_Item_Image.sprite = update_Item.GetSprite();
//        this.item = update_Item;
//        this.isIn = true;
//    }

//    private void Clear_Slot()
//    {
//        SetColor(0f, slot_Item_Image);
//        this.item = null;
//        this.slot_Item_Image.sprite = null;
//        this.isIn = false;
//    }

//    private void SetColor(float alpha, Image setImage)
//    {
//        Color color = setImage.color;
//        color.a = alpha;
//        setImage.color = color;
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        SetColor(1f, Event_Image);
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        SetColor(0f, Event_Image);
//    }

//    // Getter / Setter
//    public void SetSlotItem(Item setitem) { item = setitem; }
//    public void SetSlotImage(Image SlotImage) { slot_Item_Image = SlotImage; }
//    public void SetIsIn(bool value) { isIn = value; }
//    public void SetCountText(int count) { slot_Item_Count_TXT.text = count.ToString(); }
//    public bool IsIn() { return isIn; }
//    public Item GetItemData() { return item; }
//}
