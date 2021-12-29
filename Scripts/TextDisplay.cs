using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour
{
    public Text text;
    public GameObject btn;
    public string[] conversations;
    int i;

    void Start()
    {
        StartCoroutine(TypeSentence());
    }

    void Update()
    {
        if (text.text == conversations[i])
        {
            btn.SetActive(true);
        }
        else
        {
            btn.SetActive(false);
        }
    }

    IEnumerator TypeSentence()
    {
        foreach (char l in conversations[i].ToCharArray())
        {
            text.text += l;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void gotoNextSentence()
    {
        //ç≈å„ÇÃï™Ç‹Ç≈ìûíBÇµÇƒÇ¢Ç»Ç¢èÍçá
        if (i < conversations.Length - 1)
        {
            i++;
            text.text = "";
            StartCoroutine(TypeSentence());
        }
        else
        {
            TalkPoint.instance.talkOut();
        }
    }
}
