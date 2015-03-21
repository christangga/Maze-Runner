using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
	public int slotsX, slotsY;
	public GUISkin skin;
	public List<Item> inventory = new List<Item>();
	public List<Item> slots = new List<Item>();
	public bool showInventory;

	private ItemDatabase database;
	private bool showTooltip;
	private string tooltip;

	private bool draggingItem;
	private Item draggedItem;
	private int prevIndex;

	void Start() {
		for (int i=0; i<slotsX*slotsY; i++) {
			slots.Add(new Item());
			inventory.Add(new Item());
		}
		database = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();
		AddItem(1);
		AddItem(0);
		AddItem(0);
		AddItem(0);
		//LoadInventory();
	}

	void Update() {
		if (Input.GetButtonDown("Inventory")) {
			showInventory = !showInventory;
		}
		if (!showInventory) {
			showTooltip = false;
		}
	}

	void OnGUI() {
		SaveInventory();
		/*if (GUI.Button(new Rect(40, 400, 100, 40), "Save")) {
			SaveInventory();
		}
		if (GUI.Button(new Rect(40, 450, 100, 40), "Load")) {
			LoadInventory();
		}*/

		tooltip = "";
		GUI.skin = skin;
		if (showInventory) {
			DrawInventory();
			if (showTooltip) {
				DrawTooltip();
			}
		}
		if (draggingItem) {
			GUI.DrawTexture(new Rect(Event.current.mousePosition.x + 15f, Event.current.mousePosition.y, 30, 30), draggedItem.itemIcon);
		}
	}

	void DrawInventory() {
		Event e = Event.current;
		int pos = 0;

		for (int j=0; j<slotsY; j++) {
			for (int i=0; i<slotsX; i++) {
				Rect slotRect = new Rect(i*60, j*60, 50, 50);
				GUI.Box(slotRect, "", skin.GetStyle("Slot"));
				slots[pos] = inventory[pos];
				if (slots[pos].itemName != null) {
					GUI.DrawTexture(slotRect, slots[pos].itemIcon);
					if (slotRect.Contains(e.mousePosition)) {
						if (!draggingItem) {
							tooltip = CreateTooltip(slots[pos]);
							showTooltip = true;
						}
						if (e.isMouse && e.button == 0 && e.type == EventType.mouseDown && !draggingItem) {
							tooltip = "";
							draggingItem = true;
							prevIndex = pos;
							draggedItem = slots[pos];
							inventory[pos] = new Item();
						}
						if (e.type == EventType.mouseUp && draggingItem) {
							inventory[prevIndex] = inventory[pos];
							inventory[pos] = draggedItem;
							draggingItem = false;
							draggedItem = null;
						}
						if (e.isMouse && e.type == EventType.mouseDown && e.button == 1 && e.button != 0) {
							if (inventory[pos].itemType == Item.ItemType.Consumable) {
								print("Consumable");
								UseConsumable(pos);
								// TODO
							}
						}
					}
					if (tooltip == "") {
						showTooltip = false;
					}
				} else {
					if (slotRect.Contains(e.mousePosition)) {
						if (e.type == EventType.mouseUp && draggingItem) {
							inventory[pos] = draggedItem;
							draggingItem = false;
							draggedItem = null;
						}
					}
				}
				pos++;
			}
		}
		if (e.type == EventType.mouseUp && draggingItem) {
			inventory[prevIndex] = draggedItem;
			draggingItem = false;
			draggedItem = null;
		}
	}

	private void DrawTooltip() {
		float dynamicSize = skin.box.CalcHeight(new GUIContent(tooltip), 200);

		GUI.Box(new Rect(Event.current.mousePosition.x + 15f, Event.current.mousePosition.y, 200, dynamicSize), tooltip, skin.GetStyle("Tooltip"));
	}

	string CreateTooltip(Item item) {
		string message = "<color=#FFFFFF>";
		message += item.itemName;
		message += "</color>";
		message += "\n\n";
		message += "<color=#BAED9A>";
		message += item.itemDesc;
		message += "</color>";

		return message;
	}

	private void RemoveItem(int id) {
		for (int i=0; i<inventory.Count; i++) {
			if (inventory[i].itemName != null) {
				inventory[i] = new Item();
				break;
			}
		}
	}

	private void AddItem(int id) {
		for (int i=0; i<inventory.Count; i++) {
			if (inventory[i].itemName == null) {
				inventory[i] = database.items.Find(myItem => myItem.itemID == id);
				break;
			}
		}
	}

	private bool inventoryContains(int id) {
		return inventory.Find(myItem => myItem.itemID == id) != null;
	}
	
	private void UseConsumable(int id) {
		inventory[id] = new Item();
	}

	void SaveInventory() {
		for (int i=0; i<inventory.Count; i++) {
			PlayerPrefs.SetInt("Inventory " + i, inventory[i].itemID);
		}
	}

	void LoadInventory() {
		for (int i=0; i<inventory.Count; i++) {
			inventory[i] = PlayerPrefs.GetInt("Inventory " + i, -1) >= 0 ? database.items[PlayerPrefs.GetInt("Inventory " + i)] : new Item();
		}
	}

}
