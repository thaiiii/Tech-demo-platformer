using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject slotPrefab;
    public Transform inventoryPanel;
    private List<GameObject> activeSlots = new List<GameObject>();

    public void RefreshUI(List<InventoryManager.InventorySlot> inventorySlots)
    {
        foreach (var slot in activeSlots)
        {
            Destroy(slot);
        }
        activeSlots.Clear();

        //Create new slots
        foreach (var slotData in inventorySlots)
        {
            GameObject newSlot = Instantiate(slotPrefab, inventoryPanel);
            activeSlots.Add(newSlot);

            Image itemImage = newSlot.transform.Find("ItemImage").GetComponent<Image>();
            TextMeshProUGUI itemCountText = newSlot.transform.Find("ItemCountText").GetComponent<TextMeshProUGUI>();

            itemImage.sprite = slotData.itemSprite;
            if (slotData.isCounted)
                itemCountText.text = slotData.itemCount.ToString();
            else
                itemCountText.text = "";
        }
        HighlightSlot(FindObjectOfType<InventoryManager>().selectedSlotIndex);
    }

    public void HighlightSlot(int selectedIndex)
    {
        for (int i = 0; i < activeSlots.Count; i++)
        {
            Image slotImage = activeSlots[i].transform.Find("Background").GetComponent<Image>();
            if (i == selectedIndex)
                slotImage.color = new Color(0f, 0f, 0f, 0.6f); // Highlight slot đang chọn
            else
                slotImage.color = new Color(0f, 0f, 0f, 0.3f);  // Reset màu các slot khác
        }
    }

}
