using System;
using System.Threading;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    //スクリプト
    [SerializeField] WeaponController weaponController;

    //[SerializeField] private float radiusPixel = 20f; // レティクル許容半径（px）
    /*// カメラ内の「特定範囲」例：中央60%だけ判定
    [Range(0f, 1f)] public float minX = 0.4999f;//0.2
    [Range(0f, 1f)] public float maxX = 0.5111f;//0.8
    [Range(0f, 1f)] public float minY = 0.4999f;//0.2
    [Range(0f, 1f)] public float maxY = 0.5111f;//0.8*/

    //三種の武器(配列)
    [SerializeField] private GameObject[] weapons; //0:P320,1:EP8,2:M4

    public static int WeaponsNom;//武器の識別番号

    bool isCountShake = false;//武器の揺れ
    int countShake = 0;//武器の揺れカウント

    bool isWeaponHold = false;//武器の構え

    //プレイヤー関連
    private Rigidbody rb;//リジットボディ
    private float moveSpeed = 5.0f;//プレイヤー移動速度
    private float jumpPower = 5.0f;//ジャンプ上昇速度
    private bool isJump = false;//

    //画像
    [SerializeField] private GameObject aim; //赤い照準(AIM)

    // FPS視点パラメータ
    public Transform neck;                  // プレイヤーの首のTransformを指定
    public float sensitivity = 5.0f;     // マウス感度（視点の移動の速さを調整）
    public float minVertical = -90.0f;   // 視点の最小角度（縦の回転制限）
    public float maxVertical = 90.0f;    // 視点の最大角度（縦の回転制限）
    // 演算用変数
    private float rotationX = 0f;       // 縦方向の回転角度（首の回転）

    Camera cam;//カメラ取得(標準時ズームインの為)

    /*public enum DIRECTION_TYPE//列挙型#方向の種類
    {
        STOP,
        RIGHT,
        LEFT
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // カメラコンポーネントを取得
        cam = Camera.main;
        //GetComponent<Camera>();
        cam.fieldOfView = 60.0f; //有効視野(FOV)を小さくする(ズームイン)

        rb = GetComponent<Rigidbody>();

        WeaponsNom = 2;//武器の識別番号

        foreach (var w in weapons) w.SetActive(false);
        // 選択武器だけ表示
        int index = WeaponsNom;
        if (index >= 0 && index < weapons.Length)
            weapons[index].SetActive(true);

        //武器が○○だったらリーチやダメージ変化とかやりたい
    }

    // Update is called once per frame
    void FixedUpdate()//Rigidbodyを使った移動の処理はFixedUpdateを使う
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dir += transform.forward;
        if (Input.GetKey(KeyCode.S)) dir -= transform.forward;
        if (Input.GetKey(KeyCode.D)) dir += transform.right;
        if (Input.GetKey(KeyCode.A)) dir -= transform.right;

        // 入力がないとき normalized すると危険なので条件分岐する
        Vector3 move = Vector3.zero;
        if (dir != Vector3.zero)move = dir.normalized * moveSpeed;

        // y だけは維持
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);//velocity

        //揺れ間隔用カウント/※動いていて武器を構えていない
        if (dir != Vector3.zero && isWeaponHold == false)
        {
            isCountShake = false;
            countShake++;
            //Debug.Log("移動");
        }
        if (dir == Vector3.zero)
        {
            isCountShake = false;
            countShake = 0;//リセット
            //Debug.Log("非移動");
        }
        if (countShake >= 35)//少ししたら/
        {
            isCountShake = true;//武器の揺れ可能
            countShake = 0;//初めにリセット
        }
        if (isCountShake == true)
        {
            WeaponShake();
            Debug.Log("武器の揺れ可能");
        }
        //-------------------------------------------------------------------------
        /*
        switch (direction)
        {
            case DIRECTION_TYPE.STOP:
                speed = 0;
                break;

            case DIRECTION_TYPE.RIGHT:
                speed = 3;
                transform.localScale = new Vector3(1, 1, 1);
                break;

            case DIRECTION_TYPE.LEFT:
                speed = -3;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);//速度の変更
        */
    }

    void Update()
    {
        //マウスでカメラの視点を操作する
        // マウスの移動入力の取得
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;  // 横のマウス移動量を取得し、感度で調整
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;  // 縦のマウス移動量を取得し、感度で調整
        // Player（体）の回転（左右）
        transform.Rotate(0, mouseX, 0);   // プレイヤー（体）の左右の回転をマウスX方向の入力に合わせて行
        // Neck（首）の回転（上下）
        rotationX -= mouseY; // マウスY方向の入力によって縦方向の回転を更新
        rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);   // 回転角度を指定された範囲に制限
        neck.localRotation = Quaternion.Euler(rotationX, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space) && !isJump)//地面でスペースキーを押すと
        {
            Jump();//ジャンプ
        }

        // マウスの左クリック押した時の取得
        if (Input.GetMouseButtonDown(0))//左クリックを押すと
        {
            isWeaponHold = true;
        }
        // マウスの左クリック離した時の取得
        if (Input.GetMouseButtonUp(0))//左クリックを離すと
        {
            isWeaponHold = false;
        }
        if (Input.GetMouseButton(0))//左クリックを押している間
        {

        }

        /*if (isWeaponHold == true)//クリックしていたら
        {
            WeaponHold();//武器を構える

            if (weaponControl.IsPlayerInReticle())
            {
                //Debug.Log("プレイヤー(タグ)がカメラの特定範囲内");
                if (weaponControl.CurrentDistance > 0)
                {
                    Debug.Log("射撃！");
                }
                
            }
        }*/
        if (isWeaponHold)//クリックしていたら
        {
            WeaponHold();//武器を構える
            
            //bool hit = weaponControl.TryGiveDamage(gameObject);
            bool hit = weaponController.TryGiveDamage(transform.root.gameObject);
            if (hit)
            {
                //Debug.Log("プレイヤーにヒット！");
            }
            
        }
        else
        {
            WeaponDown();//武器を下ろす
        }

        /*float x = Input.GetAxis("Horizontal");//横方向キーの入力を取得/wasd
        //animator.SetFloat("Speed", Mathf.Abs(x)); //絶対値※Mathf

        if (x == 0)
        {
            direction = DIRECTION_TYPE.STOP;//止まっている
        }
        else if (x > 0)
        {
            direction = DIRECTION_TYPE.RIGHT;//右へ
        }
        else if (x < 0)
        {
            direction = DIRECTION_TYPE.LEFT;//左へ
        }*/

    }

    //Ground着地したらisJumpをfalseに
    private void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJump = false;
        }
    }

    public void Jump()//ジャンプ
    {
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        isJump = true;
    }

    public void WeaponShake()//武器が揺れる時の処理
    {
        //weapons[WeaponsNom].transform.localPosition = new Vector3(0.4f, 0.3f, 0.6f);//親
        //weapons[0].transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    }

    public void WeaponHold()//武器を構える時の処理
    {
        aim.SetActive(true);//赤い照準を出す
        cam.fieldOfView = 20.0f;//有効視野(FOV)を小さく(ズームイン)
        weapons[0].transform.localPosition = new Vector3(0.0f, +0.043f, 0.40f);//親+位置/※左右上下前後
        weapons[1].transform.localPosition = new Vector3(0.0f, -0.170f, 0.40f);//親+位置/※左右上下前後
        weapons[2].transform.localPosition = new Vector3(0.0f, -0.055f, 0.40f);//親+位置/※左右上下前後
        weapons[WeaponsNom].transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);//角度
    }
    public void WeaponDown()//武器を下ろす時の処理
    {
        aim.SetActive(false);//赤い照準を消す
        cam.fieldOfView = 60.0f;//有効視野(FOV)を戻す(ズームアウト)
        weapons[0].transform.localPosition = new Vector3(0.4f, 0.0f, 0.6f);//親+位置/※左右上下前後
        weapons[1].transform.localPosition = new Vector3(0.4f, 0.0f, 0.6f);//親+位置/※左右上下前後
        weapons[2].transform.localPosition = new Vector3(0.4f, 0.0f, 0.6f);//親+位置/※左右上下前後
        weapons[WeaponsNom].transform.localRotation = Quaternion.Euler(10.0f, 10.0f, 0.0f);//角度
    }

    public void UI()//他UIの処理(除く→HPバー/照準)
    {
        
    }

}
