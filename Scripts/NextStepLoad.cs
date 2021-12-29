using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStepLoad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Invoke("NextStep", 3f);
        }
    }

    public void NextStep()
    {
        SceneManager.LoadScene(0);
    }
}
