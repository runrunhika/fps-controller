using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//UIを取得する方法
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class FPSController : MonoBehaviour
{
    public FixedJoystick inputMove; //左画面JoyStick
    float moveSpeed = 6.0f; //移動する速度
    bool playerAttack;
    bool reload;
    bool aim;

    //変数宣言
    float x, z;
    // float speed = 0.1f;
    //回転取得・キャラクターの回転取得
    Quaternion cameraRot, characterRot;

    //マウス角度の制限
    float minX = -90f, maxX = 90f;

    public Animator animator;
    //所持弾薬・最高所持弾薬・マガジン内の段数・マガジン内の最大数
    int ammunition = 99, maxAmmunition = 999, ammoClip = 99, maxAmmoClip = 99;

    //体力・Max体力
    float playerHP = 100, maxPlayerHP = 100;
    public Slider hpBar;
    public Text ammoText;

    //カメラの取得
    public GameObject mainCamera, subCamera;

    //音源
    public AudioSource playerFootStep;
    public AudioClip WalkFootStepSE, RunFootStepSE;

    public AudioSource voice, impact;
    public AudioClip hitVoiceSE, hitImpactSE;

    //System
    public GameObject gameOverText;

    //Item
    public int ammoBox, medBox;

    /// <summary> カメラ操作を受け付けるタッチエリア </summary>
    [SerializeField]
    private DragHandler _lookController;

    /// <summary> カメラ速度（°/px） </summary>
    [SerializeField]
    private float _angularPerPixel = 1f;

    /// <summary> カメラ操作として前フレームにタッチしたキャンバス上の座標 </summary>
    private Vector2 _lookPointerPosPre;
    [SerializeField]
    private Camera _camera;

    /// <summary> 起動時 </summary>
    private void Awake()
    {
        _lookController.OnBeginDragEvent += OnBeginDragLook;
        _lookController.OnDragEvent += OnDragLook;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameState.canShoot = true;
        gameOverText.SetActive(false);
        GameState.GameOver = false;

        HPUpdate();
        AmmoUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameState.GameOver)
        {
            return;
        }

        if (playerAttack && GameState.canShoot)
        {
            if (ammoClip > 0)
            {
                animator.SetTrigger("Fire");
                GameState.canShoot = false;

                ammoClip--;
                AmmoUpdate();

                playerAttack = false;
                GameState.canShoot = true;
            }
            else
            {
                Weapon.instance.TriggerSE();
            }

        }

        if (reload)
        {
            playerAttack = false;
            aim = false;
            reload = false;
            int amountNeed = maxAmmoClip - ammoClip;
            //リロードする弾薬数より所持している弾薬数が多いいか
            int ammoAvailable = amountNeed < ammunition ? amountNeed : ammunition;

            if (amountNeed != 0 && ammunition != 0)
            {
                animator.SetTrigger("Reload");
                ammunition -= ammoAvailable;
                ammoClip += ammoAvailable;
                AmmoUpdate();
            }

        }

        //左スティックでの縦移動
        this.transform.position += this.transform.forward * inputMove.Vertical * moveSpeed * Time.deltaTime;
        //左スティックでの横移動
        this.transform.position += this.transform.right * inputMove.Horizontal * moveSpeed * Time.deltaTime;

        if (inputMove.Vertical > 0 || inputMove.Horizontal > 0)
        {
            animator.SetBool("Walk", true);

            //音を出す    
            //PlayerWalkFootStep(WalkFootStepSE);
        }
        else
        {
            animator.SetBool("Walk", false);

            //StopFootStep();
        }

        if (aim)
        {
            reload = false;
            //カメラの切り替え
            subCamera.SetActive(true);
            mainCamera.GetComponent<Camera>().enabled = false;
        }
        else if (subCamera.activeSelf)
        {
            subCamera.SetActive(false);
            mainCamera.GetComponent<Camera>().enabled = true;
        }
    }

    public void reloadButton()
    {
        reload = true;
    }

    public void playerAttackButtonDown()
    {
        playerAttack = true;
    }
    public void playerAttackButtonUp()
    {
        playerAttack = false;
    }

    public void aimButton()
    {
        aim = !aim;
    }

    /// カメラ操作
#region Look
    /// <summary> ドラッグ操作開始（カメラ用） </summary>
    private void OnBeginDragLook(PointerEventData eventData)
    {
        _lookPointerPosPre = _lookController.GetPositionOnCanvas(eventData.position);
    }

    /// <summary> ドラッグ操作中（カメラ用） </summary>
    private void OnDragLook(PointerEventData eventData)
    {
        var pointerPosOnCanvas = _lookController.GetPositionOnCanvas(eventData.position);
        // キャンバス上で前フレームから何px操作したかを計算
        var vector = pointerPosOnCanvas - _lookPointerPosPre;
        // 操作量に応じてカメラを回転
        LookRotate(new Vector2(-vector.y, vector.x));
        _lookPointerPosPre = pointerPosOnCanvas;
    }

    private void LookRotate(Vector2 angles)
    {
        Vector2 deltaAngles = angles * _angularPerPixel;
        transform.eulerAngles += new Vector3(0f, deltaAngles.y);
        //
        // _camera.transform.localEulerAngles += new Vector3(deltaAngles.x, 0f);
        _camera.transform.localEulerAngles += new Vector3(Mathf.Clamp(deltaAngles.x, minX, maxX), 0f);
    }
    #endregion

    //歩きのSE
    public void PlayerWalkFootStep(AudioClip clip)
    {
        playerFootStep.loop = true;
        //音の高さ
        playerFootStep.pitch = 1f;
        //音源の設定
        playerFootStep.clip = clip;
        //音を出す
        playerFootStep.Play();
    }

    //走りのSE
    public void PlayerRunFootStep(AudioClip clip)
    {
        playerFootStep.loop = true;
        //音の高さ
        playerFootStep.pitch = 1.3f;
        //音源の設定
        playerFootStep.clip = clip;
        //音を出す
        playerFootStep.Play();
    }

    //音を止める
    public void StopFootStep()
    {
        //音を止める
        playerFootStep.Stop();
        playerFootStep.loop = false;
        playerFootStep.pitch = 1f;
    }

    //HPを減らす
    public void TakeHit(float damage)
    {
        playerHP = (int)Mathf.Clamp(playerHP - damage, 0, playerHP);
        HPUpdate(); 
        ImpactSE();

        if (Random.Range(0, 10) < 6)
        {
            VoiceSE(hitVoiceSE);
        }

        if (playerHP <= 0 && !GameState.GameOver)
        {
            GameState.GameOver = true;

            gameOverText.SetActive(true);

            Invoke("Restart", 3f);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void VoiceSE(AudioClip clip)
    {
        voice.Stop();
        voice.clip = clip;
        voice.Play();
    }

    public void ImpactSE()
    {
        voice.clip = hitImpactSE;
        voice.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo")
        {
            if(maxAmmunition > ammunition)
            {
                ammunition += ammoBox;
                
                if(maxAmmunition < ammunition)
                {
                    ammunition = maxAmmunition;
                }
                AmmoUpdate();
                Destroy(other.gameObject);
            }
        }
        else if(other.tag == "Med")
        {
            if (maxPlayerHP > playerHP)
            {
                playerHP += medBox;

                if (maxPlayerHP < playerHP)
                {
                    playerHP = maxPlayerHP;
                }
                HPUpdate();
                Destroy(other.gameObject);
            }
        }
        
    }

    private void OnTriggerStay(Collider collider)
    {
        if(collider.tag == "HealthZoon")
        {
            StartCoroutine(HealthTime());
        }
    }

    IEnumerator HealthTime()
    {
        while (maxPlayerHP > playerHP)
        {
            yield return new WaitForSeconds(1);
            playerHP += Time.deltaTime * 2;
            if(maxPlayerHP <= playerHP)
            {
                playerHP = maxPlayerHP;
            }

            HPUpdate();
        }
        Debug.Log(playerHP);
    }

    public void HPUpdate()
    {
        hpBar.value = playerHP;
    }

    public void AmmoUpdate()
    {
        ammoText.text = ammoClip + "/" + ammunition;
    }
}