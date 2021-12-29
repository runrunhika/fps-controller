using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZombie : MonoBehaviour
{
    //���O�h�[������
    public bool rd;

    //Drop�m��
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

        //Enemy������
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
