using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Star : MonoBehaviour
{
    protected float Despawn;
    protected GameObject planetModel;
    protected float size;
    public float MinSize = 10;
    public float MaxSize = 100;

    // Start is called before the first frame update
    public void Start()
    {
        planetModel = transform.GetChild(0).gameObject;
        SpawnStar();
    }

    // Update is called once per frame
    public void Update()
    {
        if (Despawn > 0)
        {
            Despawn -= Time.deltaTime;
        }
    }

    public virtual void SpawnStar()
    {
        Despawn = 1;
    }


    protected void OnTriggerStay(Collider other)
    {
        if (Despawn > 0)
        {
            if (other.transform.parent.GetComponent<Star>() != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
