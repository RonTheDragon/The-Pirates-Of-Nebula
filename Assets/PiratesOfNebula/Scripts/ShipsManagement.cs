using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class ShipsManagement : MonoBehaviour
{
    public GameObject Background;
    public GameObject Spaceship;
    public GameObject TheCaptain;
    public GameObject[] Menus;
    public GameObject[] Controls;
    public GameObject WeaponsList;
    public List<GameObject> Weapons;
    public List<GameObject> AllWeaponsInGameList;
    Currency c;
    PlayerData d;
    LineRenderer LR;
    bool Alive = true;
    bool _captainOutSide;

    public GameObject GameOverPanel;

    private void Awake()
    {
        c = GetComponent<Currency>();
        LR = TheCaptain.GetComponent<LineRenderer>();
        if (PlayerPrefs.GetInt("NewGame")==0)
        {
            d = SaveSystem.Load(PlayerPrefs.GetInt("slot"));
            c.Money = d.Money; //set Money
            StartCoroutine(UpdateTheShop());
            Weapons = new List<GameObject>();
            for (int i = 0; i < d.Inventory.Length; i++) //Set Inventory
            {
                Weapons.Add(AllWeaponsInGameList[d.Inventory[i]]);
            }
            PlayerControl p = Spaceship.GetComponent<PlayerControl>();
            p.Cannons = new GameObject[d.ItemSlots.Length];
            for (int i = 0; i < d.ItemSlots.Length; i++) //Set Weapon Slots
            {
                p.Cannons[i] = AllWeaponsInGameList[d.ItemSlots[i]];
            }
            c.Level = d.Level;
            c.LevelsUnlocked = d.LevelsUnlocked;

        }
        else
        {
            c.Level = 1;
            c.LevelsUnlocked = 1;
            GetComponent<Tutorial>().StartTutorial();
            StartCoroutine(FirstColor());
        }

        GameOverPanel.SetActive(false);
        
    }

    IEnumerator FirstColor()
    {
        yield return new WaitForSeconds(0.01f);
        c.Textures[0].ChangeColor();
        c.Textures2[0].ChangeColor2();
    }

    IEnumerator UpdateTheShop()
    {
        yield return new WaitForSeconds(0.01f);

        for (int i = 0; i < c.Items.Count; i++) //Set Shop
        {
            if (d.Shop[i])
            {
                c.Items[i].AlreadyBought();
            }
        }
        for (int i = 0; i < c.Upgrades.Count; i++) //Set Shop
        {
            int count = 0;
            while (c.Upgrades[i].Level < d.Upgrades[i])
            {
                c.Upgrades[i].Upgraded();
                count++;
                if (count > 100) break;
            }
        }
        for (int i = 0; i < c.Textures.Count; i++) //Set Shop
        {
            if (d.ColorShop[i])
            {
                c.Textures[i].AlreadyBought();
            }
        }
        for (int i = 0; i < c.Textures2.Count; i++) //Set Shop
        {
            if (d.ColorShop2[i])
            {
                c.Textures2[i].AlreadyBought();
            }
        }
        for (int i = 0; i < c.Galaxies.Count; i++) //Set Shop
        {
            if (d.GalaxyShop[i])
            {
                c.Galaxies[i].AlreadyBought();
            }
        }
        c.ColorShop();
        c.UpgradeShop();
        c.GalaxyShop();
        c.Textures[d.Color].ChangeColor();
        c.Textures2[d.Color2].ChangeColor2();
    }

    void Start()
    {
        setWeaponSwitching();
  
    }
  
    void Update()
    {
        if (Spaceship.activeSelf == true)
        {
            if (Background != null)
            {
                Vector3 Map = Background.transform.position;
              if (Spaceship.transform.position.x < Background.transform.position.x-100)
                { Background.transform.position = new Vector3() { x = Map.x - 100, y = Map.y, z = Map.z }; }
              else if (Spaceship.transform.position.x > Background.transform.position.x + 100)
                    { Background.transform.position = new Vector3() { x = Map.x + 100, y = Map.y, z = Map.z }; }
              if (Spaceship.transform.position.z < Background.transform.position.z - 100)
                { Background.transform.position = new Vector3() { z = Map.z - 100, y = Map.y, x = Map.x }; }
                else if (Spaceship.transform.position.z > Background.transform.position.z + 100)
                { Background.transform.position = new Vector3() { z = Map.z + 100, y = Map.y, x = Map.x }; }
            }
            CaptainControl();
        }
        else if (Alive)
        {
            Alive = false;
            StartCoroutine(GameOver());
        }
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2);
        GameOverPanel.SetActive(true);
    }
    public void SwitchWeapons()
    {
        ChangeMenu(1);
        Time.timeScale = 0;
    }
    public void FlyShip()
    {
        ChangeMenu(0);   
        Time.timeScale = 1;
    }
    public void LeaveShop()
    {
        SaveSystem.Save(gameObject, PlayerPrefs.GetInt("slot"));
        ChangeMenu(0);
        Time.timeScale = 1;
    }
    public void VisitShop()
    {
        SaveSystem.Save(gameObject, PlayerPrefs.GetInt("slot"));
        GetComponent<Currency>().Shop();
        ChangeMenu(2);
        Time.timeScale = 0;
        c.Shop();
        c.ColorShop();
        c.UpgradeShop();
        c.GalaxyShop();
    }
    public void SaveAndExit()
    {
        SaveSystem.Save(gameObject, PlayerPrefs.GetInt("slot"));
        SceneManager.LoadScene("OpenScreen");
    }

    public void RestartGame()
    {
        SaveSystem.Save(gameObject, PlayerPrefs.GetInt("slot"));
        PlayerPrefs.SetInt("NewGame", 0);
        SceneManager.LoadScene("SampleScene");
    }

    void ChangeMenu(int Menu)
    {
        foreach(GameObject m in Menus)
        {
            m.SetActive(false);
        }
        Menus[Menu].SetActive(true);
    }
    public void setWeaponSwitching()
    {
        foreach (Transform g in WeaponsList.transform)
        {
            Destroy(g.gameObject);
        }

        foreach (GameObject w in Weapons)
        {
            Weapon c = w.GetComponent<Weapon>();
            if (c != null)
            {
                GameObject i = Instantiate(c.WeaponIcon,WeaponsList.transform, false);
                DragAndDrop d = i.GetComponent<DragAndDrop>(); 
                d.TheWeapon = w;             
            }
        }
    }
    public void CaptainHijack()
    {
        if (Spaceship.GetComponent<Health>()?.Froze == false)
        {
            if (_captainOutSide) //Back In
            {
                Spaceship.GetComponent<PlayerControl>().ComputerSupport = true;
                _captainOutSide = false;
                Controls[0].SetActive(true);
                Controls[1].SetActive(false);
            }
            else // Go Out
            {
                Spaceship.GetComponent<PlayerControl>().ComputerSupport = false;
                TheCaptain.transform.position = Spaceship.transform.position;
                TheCaptain.transform.rotation = Spaceship.transform.rotation;
                _captainOutSide = true;
                Controls[0].SetActive(false);
                Controls[1].SetActive(true);
                TheCaptain.SetActive(true);
            }
        }
    }
    void CaptainControl()
    {
        if (_captainOutSide==false&&TheCaptain.activeSelf==true)//if captain suppose to be inside
        {
            TheCaptain.transform.position = Vector3.MoveTowards(TheCaptain.transform.position, Spaceship.transform.position, 50*Time.deltaTime);

            if (Vector3.Distance(TheCaptain.transform.position, Spaceship.transform.position) < 1) { TheCaptain.SetActive(false); }
        }

        if (TheCaptain.activeSelf == true) // if The Captain Is Out
        {
            LR.SetPosition(0, TheCaptain.transform.position);
            LR.SetPosition(1, Spaceship.transform.position);

            float dist = Vector3.Distance(TheCaptain.transform.position, Spaceship.transform.position); // Distance between Captain and Ship

            float PullStrength = 1;
            if (dist > 12) { PullStrength = 2f; }
            else if (dist > 9) { PullStrength = 1.6f; }
            else if (dist > 7) { PullStrength = 1.4f; }
            else if (dist > 5) { PullStrength = 1.2f; }

            TheCaptain.transform.position = Vector3.MoveTowards(TheCaptain.transform.position, Spaceship.transform.position, dist * PullStrength * Time.deltaTime);
            
        }
    }
}
