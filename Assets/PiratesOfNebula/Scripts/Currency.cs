using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]

public abstract class Product
{
    public string Name;
    protected Currency c;
    public int Cost;
    public abstract void Buy();
    
    public void SetCurrency(Currency C)
    {
        c = C;
    }
}
[System.Serializable]
public class Upgrade : Product
{
    [HideInInspector]
    public int Level;
    public int MaxLevel;
    public float CostMultiplier = .5f;
    public Sprite Icon;
    [HideInInspector]
    public List<int> Costs;
    public UnityEvent OnUpgrade;
    public override void Buy()
    {
        if (c.Money >= Costs[Level] && Level < Costs.Count-1)
        {
            c.Money -= Costs[Level];
            OnUpgrade?.Invoke();
            Level++;
            c.UpgradeShop();
        }
    }
    public void Upgraded()
    {
        OnUpgrade?.Invoke();
        Level++;
    }
    public void SetCosts()
    {
        float CurrentCost = Cost;
        for (int i = 0; i < MaxLevel; i++)
        {
            Costs.Add((int)CurrentCost);
            CurrentCost *= 1+ CostMultiplier;
        }
    }
}
[System.Serializable]
public class Item : Product
{
    public GameObject Weapon;
    public int UnlockedAtLevel;
    [HideInInspector]
    public bool Bought;
    

    public override void Buy()
    {
        
        if (!Bought && c.Money>=Cost&&UnlockedAtLevel<=c.LevelsUnlocked)
        {
            Bought = true;
            c.Money -= Cost;
            ShipsManagement s = c.gameObject.GetComponent<ShipsManagement>();
            s.Weapons.Add(Weapon);
            s.setWeaponSwitching();
            c.Shop();
        }
    }
    public void AlreadyBought()
    {
        Bought = true;
    }
    
}
[System.Serializable]
public class Colors : Product
{
    public Color TheColor;
    public int UnlockedAtLevel;
    [HideInInspector]
    public Currency Player;
    [HideInInspector]
    public bool Used;
    public bool Bought;

    public override void Buy()
    {
        if (!Bought && c.Money >= Cost && UnlockedAtLevel <= c.LevelsUnlocked)
        {
            Bought = true;
            c.Money -= Cost;      
            c.ColorShop();
        }
    }
    public void AlreadyBought()
    {
        Bought = true;
    }

    public void ChangeColor()
    {
        MeshRenderer MR = Player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        MR.materials[0].color = TheColor;
        foreach (Colors c in Player.Textures) c.Used = false;
        Used = true;
    }
    public void ChangeColor2()
    {
        MeshRenderer MR = Player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        MR.materials[1].color = TheColor;
        foreach (Colors c in Player.Textures2) c.Used = false;
        Used = true;
    }
}

[System.Serializable]
public class Galaxy : Product
{
    public Sprite Icon;
    public string GalaxyLore;
    [HideInInspector]  
    public int GalaxyLevel;
    public int UnlockedAtLevel;
    public bool Bought;

    public override void Buy()
    {
        Bought = true;
        c.Money -= Cost;
        c.LevelsUnlocked++;
        c.Shop();
        c.UpgradeShop();
        c.ColorShop();
        c.GalaxyShop();
    }
    public void AlreadyBought()
    {
        Bought = true;
    }
    public void TravelToGalaxy()
    {
        if (c.Level != GalaxyLevel)
        {
            c.Level = GalaxyLevel;
            c.TravelToGalaxy = true;
        }
    }
}


public class Currency : MonoBehaviour
{
    ShipsManagement SM;
    Health Hp;
    PlayerControl PC;
    ObjectPooler objectPooler;
    public List<Item> Items;
    public List<Upgrade> Upgrades;
    public List<Colors> Textures;
    public List<Colors> Textures2;
    public List<Galaxy> Galaxies;
    public int Money;
    private int money=-1;
    public TMP_Text MoneyUI;
    public GameObject button;
    public GameObject Upgradebutton;
    public GameObject ColorButton;
    public GameObject GalaxyButton;
    public GameObject WeaponsList;
    public GameObject UpgradesList;
    public GameObject ColorsList;
    public GameObject ColorsList2;
    public GameObject GalaxiesList;
    public RectTransform WeaponShopContent;
    public RectTransform UpgradesShopContent;
    public RectTransform ColorsShopContent;
    public RectTransform ColorsShopContent2;
    public RectTransform GalaxiesShopContent;
    public GameObject Wormhole;
    public GameObject WormholeCompus;
    [HideInInspector]
    public int Level;
    public int LevelsUnlocked;
    public GameObject Map;
    [HideInInspector]
    public bool TravelToGalaxy;
    bool fixtravel;
    bool FixMoneySet;
    Vector3 prev;
    bool alreadyInShop;
    float distFromWormhole;

    // Start is called before the first frame update
    private void Awake()
    {
        prev = MoneyUI.gameObject.transform.localScale;
        SM = GetComponent<ShipsManagement>();
        Hp = SM.Spaceship.GetComponent<Health>();
        PC = SM.Spaceship.GetComponent<PlayerControl>();

    }

    void Start()
    {       
        objectPooler = ObjectPooler.Instance;
        foreach(Item i in Items)
        {
            i.SetCurrency(this);
        }
        foreach(Upgrade i in Upgrades)
        {
            i.SetCurrency(this);
            i.SetCosts();
        }
        foreach (Colors i in Textures)
        {
            i.SetCurrency(this);
            i.Player = this;
        }
        foreach (Colors i in Textures2)
        {
            i.SetCurrency(this);
            i.Player = this;
        }        
        for (int i = 0; i < Galaxies.Count; i++)
        {
            Galaxies[i].SetCurrency(this);
            Galaxies[i].GalaxyLevel = i+1;
        }
        foreach(Transform g in Map.transform)
        {
            g.gameObject.SetActive(false);
        }
        Map.transform.GetChild(Level).gameObject.SetActive(true);
        Shop();
        UpgradeShop();
        ColorShop();
        GalaxyShop();
        alreadyInShop = true;
      //  SpawnWormHole();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoney();
        if (WormholeCompus.activeSelf)
        {
            WormholeCompus.transform.LookAt(Wormhole.transform.position);
        }
        distFromWormhole = Vector3.Distance(SM.Spaceship.transform.position, Wormhole.transform.position);
        if (distFromWormhole < 4 && alreadyInShop == false) { alreadyInShop = true;  SM.VisitShop(); }
        else if (distFromWormhole >= 4) { alreadyInShop = false; }
     
        
        if (TravelToGalaxy)
        {
            SM.Spaceship.transform.position = Vector3.MoveTowards(SM.Spaceship.transform.position, Wormhole.transform.position, 100 * Time.deltaTime);
            SM.Spaceship.transform.Rotate(new Vector3(0,1000*Time.deltaTime,0));
            SM.Spaceship.GetComponent<Health>().Hp += 1000;
            Wormhole.transform.localScale += new Vector3(1, 1, 1) *2* Time.deltaTime;
            if (fixtravel == false)
            {
                PlayerPrefs.SetInt("NewGame", 0);
                fixtravel = true;
                SM.LeaveShop();
                StartCoroutine(MoveToGalaxy());
            }
        }
    }

    public void UpdateMoney()
    {
        if (Money != money)
        {
            money = Money;
            MoneyUI.text = string.Format("{0:#,#}$", money);
            if (FixMoneySet) StartCoroutine(MoneyEffect());
            
        }
        FixMoneySet = true;
    }

    IEnumerator MoneyEffect()
    {
        if (Time.timeScale > 0)
        {
            MoneyUI.gameObject.transform.localScale = new Vector3() { x = prev.x * 1.2f, y = prev.y * 1.2f, z = prev.z * 1.2f };
            MoneyUI.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            MoneyUI.gameObject.transform.localScale = prev;
            MoneyUI.color = Color.white;
        }
    }
    IEnumerator MoveToGalaxy()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SampleScene");
    }

    public void PopupMoney(GameObject position, int amount)
    {
       objectPooler.SpawnFromPool("MoneyPopup", position.transform.position, Quaternion.Euler(new Vector3() {x=90,y=0,z=0 })).transform.GetChild(0).GetComponent<TMP_Text>().text = $"+{amount.ToString()}"; 
        
    }
    
    public void Shop()
    {
        foreach (Transform g in WeaponsList.transform)
        {
            Destroy(g.gameObject);
        }


        int count = 0;
        //yes
        foreach (Item i in Items)
        {
            if (!i.Bought)
            {
                count++;
                GameObject g = Instantiate(button, WeaponsList.transform,false);
                TMP_Text t = g.transform.GetChild(0).GetComponent<TMP_Text>();
                t.text = $"{i.Name}";
                TMP_Text tt = g.transform.GetChild(2).GetComponent<TMP_Text>();
                tt.text = $"Costs: {i.Cost}$";
                Button b = g.GetComponent<Button>();
                b.onClick.AddListener(i.Buy);
                Image Im = g.transform.GetChild(1).GetComponent<Image>();
                Im.sprite = i.Weapon.GetComponent<Weapon>().WeaponIcon.GetComponent<Image>().sprite;
                if(i.UnlockedAtLevel>LevelsUnlocked)g.transform.GetChild(3).gameObject.SetActive(true);

            }
        }
        WeaponShopContent.sizeDelta = new Vector2(WeaponShopContent.sizeDelta.x, count * 300);
    }
    public void UpgradeShop()
    {
        foreach (Transform g in UpgradesList.transform)
        {
            Destroy(g.gameObject);
        }

        //yes
        foreach (Upgrade i in Upgrades)
        {
            if (i.Level < i.Costs.Count - 1)
            {
                
                GameObject g = Instantiate(Upgradebutton, UpgradesList.transform, false);
                TMP_Text t = g.transform.GetChild(0).GetComponent<TMP_Text>();
                t.text = $"{i.Name}";
                TMP_Text tt = g.transform.GetChild(2).GetComponent<TMP_Text>();
                tt.text = $"Costs:{i.Costs[i.Level]}$ ";
                TMP_Text ttt = g.transform.GetChild(3).GetComponent<TMP_Text>();
                ttt.text = $"lvl {i.Level + 1}";
                Button b = g.GetComponent<Button>();
                b.onClick.AddListener(i.Buy);
                Image Im = g.transform.GetChild(1).GetComponent<Image>();
                if (i.Icon!=null) Im.sprite = i.Icon;
                
            }
            else
            {
                GameObject g = Instantiate(Upgradebutton, UpgradesList.transform, false);
                TMP_Text t = g.transform.GetChild(0).GetComponent<TMP_Text>();
                t.text = $"{i.Name}";
                TMP_Text tt = g.transform.GetChild(2).GetComponent<TMP_Text>();
                tt.text = "Maxed Out";
                TMP_Text ttt = g.transform.GetChild(3).GetComponent<TMP_Text>();
                ttt.text = $"lvl {i.Level + 1}";
                Image Im = g.transform.GetChild(1).GetComponent<Image>();
                if (i.Icon != null) Im.sprite = i.Icon;
            }
        }
        UpgradesShopContent.sizeDelta = new Vector2(UpgradesShopContent.sizeDelta.x, Upgrades.Count * 300);
    }

    public void ColorShop()
    {
        foreach (Transform g in ColorsList.transform)
        {
            Destroy(g.gameObject);
        }
        foreach (Colors i in Textures)
        {         
            GameObject g = Instantiate(ColorButton, ColorsList.transform, false);
            Image Im = g.transform.GetChild(0).GetComponent<Image>();
            Im.color = i.TheColor;
            if (i.UnlockedAtLevel > LevelsUnlocked) { g.transform.GetChild(2).gameObject.SetActive(true); }
            else if (!i.Bought)
            {
                g.transform.GetChild(1).gameObject.SetActive(true);
                TMP_Text tt = g.transform.GetChild(1).GetComponent<TMP_Text>();
                tt.text = $"{i.Cost}$";
                g.GetComponent<Button>().onClick.AddListener(i.Buy);
            }
            else
            {
                g.GetComponent<Button>().onClick.AddListener(i.ChangeColor);
            }
            
        }
        int num = Textures.Count / 3;
        ColorsShopContent.sizeDelta = new Vector2(ColorsShopContent.sizeDelta.x, num * 250);


        foreach (Transform g in ColorsList2.transform)
        {
            Destroy(g.gameObject);
        }
        foreach (Colors i in Textures2)
        {
            GameObject g = Instantiate(ColorButton, ColorsList2.transform, false);
            Image Im = g.transform.GetChild(0).GetComponent<Image>();
            Im.color = i.TheColor;
            if (i.UnlockedAtLevel > LevelsUnlocked) { g.transform.GetChild(2).gameObject.SetActive(true); }
            else if (!i.Bought)
            {
                g.transform.GetChild(1).gameObject.SetActive(true);
                TMP_Text tt = g.transform.GetChild(1).GetComponent<TMP_Text>();
                tt.text = $"{i.Cost}$";
                g.GetComponent<Button>().onClick.AddListener(i.Buy);
            }
            else
            {
                g.GetComponent<Button>().onClick.AddListener(i.ChangeColor2);
            }

        }
        int num2 = Textures.Count / 3;
        ColorsShopContent2.sizeDelta = new Vector2(ColorsShopContent2.sizeDelta.x, num2 * 250);
    }

    public void GalaxyShop()
    {
        foreach (Transform g in GalaxiesList.transform)
        {
            Destroy(g.gameObject);
        }
        foreach (Galaxy i in Galaxies)
        {
            GameObject g = Instantiate(GalaxyButton, GalaxiesList.transform, false);

            Image Im = g.transform.GetChild(0).GetComponent<Image>();
            if (i.Icon != null) Im.sprite = i.Icon;

            TMP_Text t = g.transform.GetChild(1).GetComponent<TMP_Text>();
            t.text = $"{i.Name}";
            TMP_Text tt = g.transform.GetChild(2).GetComponent<TMP_Text>();
            tt.text = $"{i.GalaxyLore}";
            if (i.UnlockedAtLevel > LevelsUnlocked) { g.transform.GetChild(4).gameObject.SetActive(true); }
            else if (!i.Bought)
            {
                g.transform.GetChild(3).gameObject.SetActive(true);
                TMP_Text ttt = g.transform.GetChild(3).GetComponent<TMP_Text>();
                ttt.text = $"Costs: {i.Cost}$";
                g.GetComponent<Button>().onClick.AddListener(i.Buy);
            }
            else
            {
                g.GetComponent<Button>().onClick.AddListener(i.TravelToGalaxy);
            }

        }
        ColorsShopContent.sizeDelta = new Vector2(ColorsShopContent.sizeDelta.x, Galaxies.Count*300);
    }

    public void SpawnWormHole()
    {
        float x = Random.Range(100, 200 + 1);
        if (Random.Range(0, 2) == 0) { x *= -1; }
        float z = Random.Range(100, 200 + 1);
        if (Random.Range(0, 2) == 0) { z *= -1; }
        float r = Random.Range(0, 361);
        Wormhole.transform.position = new Vector3(SM.Spaceship.transform.position.x + x, SM.Spaceship.transform.position.y, SM.Spaceship.transform.position.z + z);
    }
    
    public void TurnCompus()
    {
        if (WormholeCompus.activeSelf) { WormholeCompus.SetActive(false); }
        else { if (distFromWormhole > 350) SpawnWormHole(); WormholeCompus.SetActive(true); } 
    }
   
    


    public void UpgradeHealth()
    {
        Hp.MaxHps[Hp.Phase] += 40;
        Hp.Armor += 5;
        Hp.Hp = Hp.MaxHps[Hp.Phase];
    }
    public void UpgradeSpeed()
    {
       
        PC.MovementSpeed += 2;
    }
    public void UpgradeRaids()
    {
        
        PC.MilkTime -= 0.4f;
        PC.PiratesAmount++;
    }
    public void UpgradeHealing()
    {
        Hp.MaxShields[Hp.Phase] += 20;
        Hp.ShieldRegan+=8;
        Hp.ShieldCooldown -= 0.2f;
        Hp.Shield = Hp.MaxShields[Hp.Phase];
    }
    public void UpgradeMotor()
    {
        
        PC.MaxHeat += 25;
        PC.CoolingHeat += 10;
    }
}
