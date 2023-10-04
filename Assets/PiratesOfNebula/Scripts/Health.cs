using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [HideInInspector]
    public int Phase;   
    [HideInInspector]
    public float Hp;
    public float MaxHp;
    public List<float> MaxHps;
    public float Armor;
    [HideInInspector]
    public float Shield;  
    public float MaxShield;
    public List<float> MaxShields;
    public float ShieldRegan;
    public float ShieldCooldown = 3;
    public UnityEvent OnNextPhase;
    public Image Healthbar;
    public Image Shieldbar;
    public Image Radiationbar;
    float knockback;
    GameObject Attacker;
    Vector3 V3Knockback;
    Vector3 no = new Vector3();
    public string[] SpawnOnDeath;
    public GameObject[] TurnOffWhenDeath;
    public GameObject DamageIndicator;
    public bool NoTempeture;
   [HideInInspector]
    public float Tempeture;
    [HideInInspector]
    public float Radiation;
    [HideInInspector]
    public bool Burn;
    [HideInInspector]
    public bool Froze;
    SpaceshipsAI SAI;
    ObjectPooler objectPooler;
    GameObject OnFire;
    GameObject IceCube;
    public bool CantKnockback;
    float _timeSinceTakingDamage;

    public Hookable hookable;

    // Start is called before the first frame update

    private void Awake()
    {
        AddToButtomOfList<float>(MaxHp,ref MaxHps); //Setting Up The Health Bars
        AddToButtomOfList<float>(MaxShield, ref MaxShields); //Setting Up The Shield Bars
    }

    void Start()
    {
        objectPooler = ObjectPooler.Instance;      
        if (MaxHps.Count < MaxShields.Count) { Debug.LogWarning(this.gameObject+" has more shield bars than health bars"); }
        for (int i = 0; i < 10; i++)
        {
            if (MaxHps.Count > MaxShields.Count) //if there are more hp bars than shield bars
            { MaxShields.Add(0); }
            else break;
        }
        Spawn();
        SAI = gameObject.GetComponent<SpaceshipsAI>();


    }

    // Update is called once per frame
    void Update()
    {
        if (Hp <= 0) //if dead
        { if (MaxHps.Count > Phase + 1) // if has more Healthbars
            {
                Phase++;
                Hp = MaxHps[Phase];
                Shield = MaxShields[Phase];
                OnNextPhase?.Invoke();
            }
            else Death();
        }
        else if (Hp > MaxHps[Phase]) { Hp = MaxHps[Phase]; } //if too much hp... go back to max

        _timeSinceTakingDamage += Time.deltaTime;

        if (Shield > MaxShields[Phase]) { Shield = MaxShields[Phase]; } //if too much Shield... go back to max
        else if (ShieldRegan > 0 && Shield < MaxShields[Phase] && ShieldCooldown<_timeSinceTakingDamage) { Shield += ShieldRegan * Time.deltaTime; } //Shield Regan

        if (Radiation > 0)
        {
            float rad = 1 - (Radiation * 0.01f);
            if (Hp > (MaxHps[Phase] * rad)) { Hp = MaxHps[Phase] * rad; }
            if (Shield > (MaxShields[Phase] * rad)) { Shield = MaxShields[Phase] * rad; }
        }

        if (knockback > 0) { Knockback(); }

        if (!NoTempeture)
        {
            if (Tempeture > 0)
            {
                Froze = false; Tempeture -= 20 * Time.deltaTime; IceCube?.SetActive(false);
                if (Tempeture > 100) { Tempeture = 100; if (!Burn && Hp > 0) { OnFire = objectPooler.SpawnFromPool("OnFire", transform.position, Quaternion.Euler(new Vector3()), transform.localScale, true); Burn = true; } }
            }
            else if (Tempeture < 0)
            {
                Burn = false; Tempeture += 20 * Time.deltaTime; OnFire?.SetActive(false); 
                if (Tempeture < -100) { Tempeture = -100; if (!Froze&&Hp>0) { IceCube = objectPooler.SpawnFromPool("IceCube", transform.position, transform.rotation, transform.localScale, true); Froze = true; } } }

            else { Burn = false; Froze = false; OnFire?.SetActive(false); IceCube?.SetActive(false); }

            if (Burn)
            {
                OnFire.transform.position = transform.position;
                _timeSinceTakingDamage = 0;
                if (Shield > 0) {Shield -= 5 * Time.deltaTime; }
                else if (Hp > 0) {Hp -= 20* Time.deltaTime; }
            }
            if (Froze)
            {
                IceCube.transform.position = transform.position;
                IceCube.transform.rotation = transform.rotation;
            }
        }

        if (Healthbar != null)
        {
            if (Phase == 0)
            {
                if (Hp > MaxHps[Phase] / 2)
                    Healthbar.color = Color.Lerp(Color.yellow, Color.green, Hp / MaxHps[Phase] * 2 - 1);
                else
                    Healthbar.color = Color.Lerp(Color.red, Color.yellow, Hp / MaxHps[Phase] * 2);
            }
            else Healthbar.color = Color.Lerp(Color.red, Color.black, 0.5f);
               
                Healthbar.fillAmount = Hp / MaxHps[Phase];
            
        }
        if (Shieldbar != null)
        {
            Shieldbar.color = Color.Lerp(Color.blue, Color.white, 0.5f);
            Shieldbar.fillAmount = Shield / MaxShields[Phase];
        }
        if (Radiationbar != null)
        {
            Radiationbar.color = Color.Lerp(Color.yellow, Color.gray, 0.5f);
            Radiationbar.fillAmount = Radiation / 100;
        }

    }

    public void Spawn()
    {
        Hp = MaxHps[Phase];
        Shield = MaxShields[Phase];
    }

    void Knockback()
    {
        if (!CantKnockback)
        {
            if (Attacker != null)
            {
                if (V3Knockback == no) { V3Knockback = new Vector3(Attacker.transform.position.x, gameObject.transform.position.y, Attacker.transform.position.z); }
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, V3Knockback, -knockback * Time.deltaTime);
            }
            knockback -= Time.deltaTime * knockback;
            knockback -= Time.deltaTime;
        }
    }


    public void Damage(float damage, float kb, GameObject attacker ,float TempChange = 0,float RadDamage = 0)
    {
        if (Hp > 0) //only work if has hp
        {
            if (damage > 0)
            {
                _timeSinceTakingDamage = 0;

                DamageMath(damage);
                //Damage Math

                Radiation += RadDamage;

                if (TempChange != 0) { if (TempChange > 0 || !Froze) Tempeture += TempChange; } //Tempeture Math
                Attacker = attacker; // Set Attacker
                if (knockback < kb) //knockback Math
                {
                    knockback = kb;
                    V3Knockback = no;
                }
                SAI?.Scan(100); //Scan For Targets after being damaged (AI only)
                if (DamageIndicator != null) StartCoroutine(OuchInducator());   //Damage Inducation     
            }
            else if (damage < 0)
            {
                Hp += -damage;
            }
        }
    }

    void Death()
    {
        RemoveElements();
        foreach (string s in SpawnOnDeath)
        {
            objectPooler.SpawnFromPool(s, transform.position, transform.rotation, transform.localScale);
        }
        foreach (GameObject g in TurnOffWhenDeath)
            g.SetActive(false);
    }

    public void RemoveElements()
    {
        OnFire?.SetActive(false);
        IceCube?.SetActive(false);
    }

    IEnumerator OuchInducator()
    {
        if (Shield > 0)
        {
            GetComponent<AudioManager>()?.PlaySound(Sound.Activation.Custom, "SOuch");
            foreach(Transform t in DamageIndicator.transform)
            {
                t.GetComponent<Image>().color = Color.blue;
            }
        }
        else
        {
            GetComponent<AudioManager>()?.PlaySound(Sound.Activation.Custom, "Ouch");
            foreach (Transform t in DamageIndicator.transform)
            {
                t.GetComponent<Image>().color = Color.red;
            }
        }
        DamageIndicator.SetActive(true);
        Healthbar.color = Color.white;
        yield return new WaitForSeconds(0.02f);
        DamageIndicator.SetActive(false);
        Healthbar.color = Color.white;
        yield return new WaitForSeconds(0.02f);
        DamageIndicator.SetActive(true);
        Healthbar.color = Color.white;
        yield return new WaitForSeconds(0.02f);
        DamageIndicator.SetActive(false);
        Healthbar.color = Color.white;
        yield return new WaitForSeconds(0.02f);
        DamageIndicator.SetActive(true);
        Healthbar.color = Color.white;
        yield return new WaitForSeconds(0.02f);
        DamageIndicator.SetActive(false);
        Healthbar.color = Color.white;
    }

    public void AddToButtomOfList<T>(T f,ref List<T> list)
    {
        List<T> NewList = new List<T>();
        NewList.Add(f);
        foreach(T t in list)
        {
            NewList.Add(t);
        }
        list = NewList;
    }

    public void DamageMath(float damage)
    {
        float damageLeft = damage;
        if (Shield > 0) // Shield Math
        {
            damageLeft -= Shield;
            Shield -= damage;
            if (Shield < 0) { Shield = 0; }
            if (damageLeft < 0) { damageLeft = 0; }
        }
        if (Armor > 0) { Hp -= damageLeft * ((100 - Armor) * 0.01f); } // Armor math
        else { Hp -= damageLeft; }
    }
}
