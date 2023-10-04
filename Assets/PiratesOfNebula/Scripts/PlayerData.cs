using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public int Money;
    public bool[] Shop;
    public int[] Upgrades;
    public int[] Inventory;
    public int[] ItemSlots;
    public bool[] ColorShop;
    public bool[] ColorShop2;
    public bool[] GalaxyShop;
    public int Color;
    public int Color2;
    public int Level;
    public int LevelsUnlocked;
    
    public PlayerData(GameObject Player)
    {
        Currency c = Player.GetComponent<Currency>(); // Set Money and Shop
        Money = c.Money;
        Shop = new bool[c.Items.Count];
        for (int i = 0; i < c.Items.Count; i++)
        {
            Shop[i] = c.Items[i].Bought;
        }
        Upgrades = new int[c.Upgrades.Count];
        for (int i = 0; i < c.Upgrades.Count; i++)
        {
            Upgrades[i] = c.Upgrades[i].Level;
        }
        ColorShop = new bool[c.Textures.Count];
        for (int i = 0; i < c.Textures.Count; i++)
        {
            ColorShop[i] = c.Textures[i].Bought;
        }
        ColorShop2 = new bool[c.Textures2.Count];
        for (int i = 0; i < c.Textures2.Count; i++)
        {
            ColorShop2[i] = c.Textures2[i].Bought;
        }
        for (int i = 0; i < c.Textures.Count; i++)
        {
            if (c.Textures[i].Used) { Color = i; }
        }
        for (int i = 0; i < c.Textures2.Count; i++)
        {
            if (c.Textures2[i].Used) { Color2 = i; }
        }
        GalaxyShop = new bool[c.Galaxies.Count];
        for (int i = 0; i < c.Galaxies.Count; i++)
        {
            GalaxyShop[i] = c.Galaxies[i].Bought;
        }

        ShipsManagement s = Player.GetComponent<ShipsManagement>(); //Organize Inventory
        Inventory = new int[s.Weapons.Count];
        for (int i = 0; i < s.Weapons.Count; i++)
        { 
            for (int j = 0; j < s.AllWeaponsInGameList.Count; j++)
            {
                if (s.Weapons[i].GetComponent<Weapon>().Id == s.AllWeaponsInGameList[j].GetComponent<Weapon>().Id) {   Inventory[i] = j; break; }
            }
        }

        PlayerControl p = s.Spaceship.GetComponent<PlayerControl>(); //Set Weapons Slots
        ItemSlots = new int[p.Cannons.Length];
        for (int i = 0; i < p.Cannons.Length; i++)
        {
            for (int j = 0; j < s.AllWeaponsInGameList.Count; j++)
            {
                if (p.Cannons[i].GetComponent<Weapon>().Id == s.AllWeaponsInGameList[j].GetComponent<Weapon>().Id) { ItemSlots[i] = j;  break; }
            }
        }
        Level = c.Level;
        LevelsUnlocked = c.LevelsUnlocked;
    }
}
