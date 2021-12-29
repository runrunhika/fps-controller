using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZombie : MonoBehaviour
{
    //ラグドール判定
    public bool rd;

    //Drop確率
    public int dorpToChance;
    //DropItem
    public GameObject[] items;

    private bool Judgment;

    // Start is called before the first frame update
    void Start()
    {
        Judgment = true;

        if(rd)
        {
            DeadZombie();
        }
    }

    public void DeadZombie()
    {
        if(Judgment)
        {
            Judgment = false;
        }
        else
        {
            return;
        }

        //Enemyを消す
        //Invoke("DestroyGameObject", 15f);

        if(Random.Range(0, 100) < dorpToChance)
        {
            Instantiate(items[Random.Range(0, items.Length)], transform.position, transform.rotation);
        }
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
