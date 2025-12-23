using NUnit.Framework.Internal;
using UnityEngine;
using System.Collections;//IEnumerator WeaponReload()//リロード時の処理で使う

public class WeaponController : MonoBehaviour
{
    //スクリプト
    [SerializeField] PlayerController playerController;

    //照準の自動トリガー
    [SerializeField] private Camera fpsCamera;//カメラ取得
    [SerializeField] private float rayDistance = 100f;//光線距離(照準判定の最大距離)
    //public float CurrentDistance { get; private set; }// 交戦距離を外から参照できるようにする
    public float DamageForce { get; private set; }//威力減衰を含めたダメージを外から参照できるようにする
    float DamageTemporary;//仮ダメージ※最小ダメージなどの調整前

    public float[] weapons_states { get; private set; }//武器ごとのステータスを配列化

    float nextFireTime = 0f;//次に射撃可能なゲーム時間
    public float restFireTime { get; private set; } = 0f;//射撃可能までの残り時間

    float nextReloadTime = 0f;//リロード完了なゲーム時間
    public float restReloadTime { get; private set; } = 0f;//リロード完了までの残り時間
    bool nowReload = false;//リロード中かどうか
    public float nowBullets { get; private set; } = 0f;//残弾数/装弾数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerController.WeaponsNom == 0)//ハンドガン※選択武器によってステータスを変える
        {
            weapons_states = new float[7];
            {
                weapons_states[0] = 16;//最大ダメージ
                weapons_states[1] = 2;//最小ダメージ
                weapons_states[2] = 2f;//威力減衰
                weapons_states[3] = 0.75f;//射撃間隔(0.75秒想定)
                weapons_states[4] = 10;//装弾数/bullets
                weapons_states[5] = 2;//再装填時間
                weapons_states[6] = 1f;//反動
            }
        }
        else if (PlayerController.WeaponsNom == 1)//サブマシンガン※選択武器によってステータスを変える
        {
            weapons_states = new float[7];
            {
                weapons_states[0] = 6;//最大ダメージ
                weapons_states[1] = 2;//最小ダメージ
                weapons_states[2] = 4f;//威力減衰
                weapons_states[3] = 0.1f;//射撃間隔(0.1秒想定)
                weapons_states[4] = 30;//装弾数/bullets
                weapons_states[5] = 6;//再装填時間
                weapons_states[6] = 1f;//反動
            }
        }
        else if (PlayerController.WeaponsNom == 2)//ショットガン※選択武器によってステータスを変える
        {
            weapons_states = new float[7];
            {
                weapons_states[0] = 75;//最大ダメージ
                weapons_states[1] = 10;//最小ダメージ
                weapons_states[2] = 8f;//威力減衰
                weapons_states[3] = 1.5f;//射撃間隔(1.5秒)
                weapons_states[4] = 6;//装弾数/bullets
                weapons_states[5] = 4;//再装填時間
                weapons_states[6] = 3f;//反動
            }
        }

        nowBullets = weapons_states[4];

    }

    // Update is called once per frame
    void Update()
    {
        restFireTime = nextFireTime - Time.time;//射撃可能までの残り時間更新

        restReloadTime = nextReloadTime - Time.time;//リロード完了までの残り時間更新

        //Rを押した時
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(nowReload == false && nowBullets != weapons_states[4])//リロード中ではない＆残弾数MAXではない時
            {
                //強制的にリロード
                StartCoroutine(WeaponReload());//リロード時間(1f = 1秒)待つ
                //Debug.Log("アローリ！");
            }
        }
        if (nowBullets <= 0 && nowReload == false)//残弾数が0以下かつリロード中でない時
        {
            StartCoroutine(WeaponReload());//リロード時間(1f = 1秒)待つ
        }

        //Debug.Log(CurrentDistance);//※相手プレイヤータグとの距離が正確化か測るため
        //Debug.Log(restFireTime);
        //Debug.Log(nextFireTime);//※次に射撃できる時間
        //Debug.Log(Time.time);
    }

    public bool TryGiveDamage(GameObject owner)//ダメージを与えるかどうかの処理
    {
        if(nowBullets <= 0　|| nowReload == true)//残弾数が0以下またはリロード中の時は
        {
            return false;//撃てない
        }

        //射撃間隔チェック
        if (Time.time < nextFireTime)//撃った時間 + 射撃間隔を超えていれば
            return false;

        //メイン処理
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Transform root = hit.collider.transform.root;

            if (!root.CompareTag("Player"))
                return false;

            if (root.gameObject == owner)
                return false;

            HpController dmg = root.GetComponentInChildren<HpController>();
            if (dmg == null)
                return false;

            float damage = CalcDamage(hit.distance);
            dmg.TakeDamage(damage);

            //次に撃てる時間を更新
            nextFireTime = Time.time + weapons_states[3];//撃った時間 + 射撃間隔

            nowBullets--;

            return true;
        }

        return false;
    }

    float CalcDamage(float distance)//威力減衰などの与えるダメージ
    {
        float maxDamage = weapons_states[0];
        float minDamage = weapons_states[1];
        float attenuation = weapons_states[2];

        float d = Mathf.Clamp01(distance / rayDistance);
        float damage = Mathf.Floor(Mathf.Lerp(maxDamage, minDamage, d * attenuation));//小数点は切り捨て※0になる可能性…こことは別の場所で何とかする
        //float damage = Mathf.Lerp(maxDamage, minDamage, d * attenuation);

        DamageTemporary = damage;

        if (damage < weapons_states[1])//最小ダメージより低い場合
        {
            damage = weapons_states[1];//最小ダメージにする※念のため
        }

        DamageForce = damage;//威力減衰を含めたダメージを外から参照できるようにする
        return damage;
    }

    public bool IsPlayerGiveDamage()
    {
        return true;
    }

    IEnumerator WeaponReload()//再装填時の処理
    {
        nowReload = true;
        //Debug.Log("リロード 開始");

        nextReloadTime = Time.time + weapons_states[5];//現在のゲーム時間+リロード時間(※別のスクリプトの処理で使うもの)

        yield return new WaitForSeconds(weapons_states[5]);//リロード時間(1f=1秒)待つ
        nowBullets = weapons_states[4];//残弾数をMAXに上書き

        nowReload = false;
        //Debug.Log("リロード 完了");

        //※StartCoroutine(WeaponReload());//リロード時間(1f = 1秒)待つ
    }

}
