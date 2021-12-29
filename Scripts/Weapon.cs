using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    //変数宣言
    public AudioSource weapon;
    public AudioClip relodingSE, fireSE, triggerSE;

        //AKM＜Cube（弾）を格納する
    public Transform shotDirection;

    //static　どこからでも呼べる
    public static Weapon instance;
    //KillEffect
    public GameObject killEffect;

    //音源
    public AudioSource killAudio;
    public AudioClip killClip;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        Debug.DrawRay(shotDirection.position, shotDirection.transform.forward * 10, Color.red);
    }

    public void CanShot()
    {
        GameState.canShoot = true;
    }

    public void FireSE()
    {
        weapon.clip = fireSE;
        weapon.Play();
    }

    public void RelodingSE()
    {
        weapon.clip = relodingSE;
        weapon.Play();
    }

    public void TriggerSE()
    {
        //音が鳴っていないとき
        if (!weapon.isPlaying)
        {
            weapon.clip = triggerSE;
            weapon.Play();
        }
    }

    public void Shooting()
    {
        RaycastHit hitInfo;
        //レーザーを飛ばし、何かに当たったかを判定  out = noValue でも引数として使える
        if(Physics.Raycast(shotDirection.transform.position, shotDirection.transform.forward, out hitInfo, 100000))
        {
            GameObject hitGameObject = hitInfo.collider.gameObject;

            if(hitInfo.collider.gameObject.GetComponent<ZombieController>() != null)
            {
                ZombieController hitZombie = hitInfo.collider.gameObject.GetComponent<ZombieController>();
                if(Random.Range(0,2) < 1)
                {
                    hitZombie.ZombieDeath();
                    GameObject rdPrefab = hitZombie.ragdoll;
                    GameObject NewRD = Instantiate(rdPrefab, hitGameObject.transform.position, hitGameObject.transform.rotation);
                    NewRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(shotDirection.transform.forward * 20000);
                    Destroy(hitGameObject);
                    
                }
                else
                {
                    hitZombie.ZombieDeath();
                }
                //KillEffect
                StartCoroutine(KE());
            }
        }
    }

    IEnumerator KE()
    {
        killEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        KillSound(killClip);
        yield return new WaitForSeconds(2);
        killEffect.SetActive(false);
    }

    public void KillSound(AudioClip clip)
    {
        killAudio.clip = clip;
        for (int i = 0; i < 2; i++)
        {
            killAudio.Play();
        }
        
    }
}
