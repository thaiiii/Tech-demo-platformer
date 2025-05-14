using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class InventorySlot
    {
        public Sprite itemSprite;
        public string itemName;
        public int itemCount;
        public bool isCounted;

        public InventorySlot() { }
        public InventorySlot(Sprite itemSprite, string itemName, int itemCount, bool isCounted)
        {
            this.itemSprite = itemSprite;
            this.itemName = itemName;
            this.itemCount = itemCount;
            this.isCounted = isCounted;
        }
    }

    public int selectedSlotIndex = -1; //-1 là chưa được chọn
    private float timeBetweenUses = 0.25f; // Thời gian giữa các lần dùng item (0.25 giây)
    private float _timeSinceLastUse;
    public float timeSinceLastUse
    {
        get { return _timeSinceLastUse; }
        set { _timeSinceLastUse = Math.Clamp(value, 0f, 5f); }
    } // Thời gian đã trôi qua kể từ lần dùng item cuối cùng, giới hạn giá trị tới 5s
    public bool isFull = false;

    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public List<InventorySlot> savedInventorySlots = new List<InventorySlot>();
    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isFull = false;
        selectedSlotIndex = -1;
        inventorySlots.Clear();
        savedInventorySlots.Clear();
        timeSinceLastUse = 5f;

        if (SceneManager.GetActiveScene().name == "MainMenu")
            player = null;
        else if (SceneManager.GetActiveScene().name == "SettingsMenu")
        {

        }   
        else if (SceneManager.GetActiveScene().name == "StartMenu")
        {

        }
        else
            player = FindAnyObjectByType<Player>().gameObject;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);

        timeSinceLastUse += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q))
            UseSelectedItem();
        if (Input.GetKey(KeyCode.Q)) UseSelectedItem();
    }


    private void SelectSlot(int slotIndex)
    {
        if (slotIndex < inventorySlots.Count)
        {
            selectedSlotIndex = slotIndex;
            InventoryUI.Instance.HighlightSlot(slotIndex);
        }
    }
    private void UseSelectedItem()
    {
        if (timeSinceLastUse >= timeBetweenUses)
        {
            timeSinceLastUse = 0f;

            if (selectedSlotIndex == -1 || selectedSlotIndex >= inventorySlots.Count)
            {
                return;
            }
            InventorySlot selectedItem = inventorySlots[selectedSlotIndex];
            if (player.GetComponent<PlayerAbilities>().isNormalStatus())
            {
                if (player.GetComponent<PlayerAbilities>().UseItem(selectedItem)) //Su dung item de dung skill
                {
                    RemoveItem(selectedItem);
                    UpdateUI();
                }
            }
        }


    }


    //Add item into inventory
    public void AddItem(Sprite itemSprite, string itemName, int itemCount, bool isCounted)
    {
        if (inventorySlots.Count == 3)
        {
            isFull = true;
            foreach (var slot in inventorySlots)
            {
                if (slot.itemName == itemName)
                    isFull = false;
            }
        }
        if (isFull)
        {
            Debug.Log("Full inven");
            return;
        }
        foreach (var slot in inventorySlots)
        {
            if (slot.itemName == itemName)
            {
                slot.itemCount += itemCount;
                UpdateUI();
                return;
            }
        }

        InventorySlot newItem = new InventorySlot(itemSprite, itemName, itemCount, isCounted);
        inventorySlots.Add(newItem);
        UpdateUI();

    }
    public void RemoveItem(InventorySlot selectedItem)
    {
        if (selectedItem.isCounted == false)
            return;

        foreach (var slot in inventorySlots)
        {
            if (slot.itemName == selectedItem.itemName)
            {
                slot.itemCount -= 1;
                if (slot.itemCount <= 0)
                {
                    inventorySlots.Remove(slot);
                    isFull = false;
                }
                UpdateUI();
                return;
            }
        }
        
    }
    public void UpdateUI()
    {
        InventoryUI.Instance.RefreshUI(inventorySlots);
    }
    public void SaveInventory()
    {
        savedInventorySlots.Clear();
        foreach (var slot in inventorySlots)
        {
            savedInventorySlots.Add(new InventorySlot(slot.itemSprite, slot.itemName, slot.itemCount, slot.isCounted));
        }
    }
    public void LoadSavedInventory()
    {
        inventorySlots.Clear();
        foreach (var slot in savedInventorySlots)
        {
            inventorySlots.Add(new InventorySlot(slot.itemSprite, slot.itemName, slot.itemCount, slot.isCounted));
        }
        UpdateUI();
    }


}
