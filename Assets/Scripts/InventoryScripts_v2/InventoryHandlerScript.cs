﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class InventoryHandlerScript : MonoBehaviour {

    private static InventoryHandlerScript instance;

    //INTERFACES
    [SerializeField]
    private  Inventory playerInventory;
    [SerializeField]
    private  Hotbar playerHotbar;

    //PANEL REFERENCES
    private  GameObject inventoryPanel;
    private  GameObject hotbarPanel;

    private  GameObject[] pInventorySlots;
    public  GameObject inventorySlotItemPrefab;

    //SINGLETON MAKE SURE ONLY ONE OF THE INSTANCES EXIST
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        ItemDictionary.GenerateDictionary();
        pInventorySlots = GetComponent<PlayerInventoryPanelScript>().slots;
        playerInventory.AddItemToIndex(ItemDictionary.GetItem(1).itemDescription, 1, 15);   //TEST
        UpdateAllPlayerInventoryPanelSlots();
    }

    //CALLED BY GAMEMANAGER TO SET ALL REFERENCES
    public void SetInventoryHandler(
        Inventory pInv,
        Hotbar pHbar,
        GameObject iPanel,
        GameObject hbarPanel
        )
    {
        playerInventory = pInv;
        playerHotbar = pHbar;
        inventoryPanel = iPanel;
        hotbarPanel = hbarPanel;

        //fetch reference to slots
        pInventorySlots = GetComponent<PlayerInventoryPanelScript>().slots;
        UpdateAllPlayerInventoryPanelSlots();
    }

    public void AddItemToPlayerInventory()
    {
        CompleteItem item = ItemDictionary.GetItem(1);
        playerInventory.AddItem(item.itemDescription, 1, this, 0);
        UpdateAllPlayerInventoryPanelSlots();
    }

    public void AddItemToPlayerHotBar()
    {

    }

    public bool ChangeItemSlot(ushort originalSlotIndex, ushort newSlotIndex)
    {
        return true;
    }

    public void UpdateAllPlayerInventoryPanelSlots()
    {
        //go through each inventory index
        for (ushort i = 0; i < playerInventory.GetInventorySize(); ++i)
        {
            //check if that index is holding nothing
            if(playerInventory.FetchItemIdInInventorySlot(i) != 0){
                //check if existing slotitem has same id
                if (pInventorySlots[i].transform.childCount > 0 && pInventorySlots[i].transform.GetChild(0).GetComponent<InventoryItem>().completeItem.itemDescription.id == playerInventory.FetchItemIdInInventorySlot(i)){
                    //just update the text
                    pInventorySlots[i].transform.GetChild(0).GetComponent<InventoryItem>().GetComponentInChildren<Text>().text = playerInventory.FetchItemAmountInInventorySlot(i).ToString();
                }
                else{
                    //if there is a invoitem in the slot destroy it first
                    if(pInventorySlots[i].transform.childCount > 0)Destroy(pInventorySlots[i].transform.GetChild(0));
                    CompleteItem item = ItemDictionary.GetItem(playerInventory.FetchItemIdInInventorySlot(i));
                    if(item != null){
                        //making a new invo item
                        GameObject invSlotItem = Instantiate(inventorySlotItemPrefab);
                        invSlotItem.GetComponent<InventoryItem>().completeItem = item;
                        invSlotItem.GetComponentInChildren<Text>().text = playerInventory.FetchItemAmountInInventorySlot(i).ToString();
                        invSlotItem.GetComponent<Image>().sprite = item.icon;
                        invSlotItem.transform.SetParent(pInventorySlots[i].transform, false);
                    }
                }
            }
        }
    }
     
    public void UpdatePlayerInventorySlot(ushort slotIndex)
    {
        ushort idAtSlot = playerInventory.FetchItemIdInInventorySlot(slotIndex);
        ushort amntAtSlot = playerInventory.FetchItemAmountInInventorySlot(slotIndex);
        GameObject inventoryItem;
        if (idAtSlot == 0)
        {
            if(pInventorySlots[slotIndex].transform.childCount > 0)
            {
                Destroy(pInventorySlots[slotIndex].transform.GetChild(0));
            }
        }
        else if(pInventorySlots[slotIndex].transform.childCount > 0)
        {
            inventoryItem = pInventorySlots[slotIndex].transform.GetChild(0).gameObject;
            if (inventoryItem.GetComponent<InventoryItem>().completeItem.itemDescription.id == idAtSlot)
            {
                inventoryItem.GetComponentInChildren<Text>().text = amntAtSlot.ToString();
            }
            else
            {
                CompleteItem item = ItemDictionary.GetItem(idAtSlot);
                inventoryItem.GetComponent<InventoryItem>().completeItem = item;
                inventoryItem.GetComponent<Image>().sprite = item.icon;
                inventoryItem.GetComponentInChildren<Text>().text = amntAtSlot.ToString();
            }
        }
    }


    public void UpdatePlayerHotbar()
    {

    }

    //SHOULD CHECK BOTH INVO AND HOTBAR
    public bool RemoveItemFromPlayer()
    {
        return true;
    }
}
