using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TalkPoint : MonoBehaviour
{
    public static TalkPoint instance;

    [SerializeField] GameObject talkPanel;

    [SerializeField] GameObject amT, hpS, r, f, fl, a;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            amT.SetActive(false);
            hpS.SetActive(false);
            r.SetActive(false);
            f.SetActive(false);
            fl.SetActive(false);
            a.SetActive(false);
            talkPanel.SetActive(true);
        }
    }

    public void talkOut()
    {
        talkPanel.SetActive(false);
        amT.SetActive(true);
        hpS.SetActive(true);
        r.SetActive(true);
        f.SetActive(true);
        fl.SetActive(true);
        a.SetActive(true);
    }

}
