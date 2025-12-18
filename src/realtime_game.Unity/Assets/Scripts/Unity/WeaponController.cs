using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //スクリプト
    [SerializeField] PlayerController playerController;

    //照準の自動トリガー
    [SerializeField] private Camera fpsCamera;//カメラ取得
    [SerializeField] private float rayDistance = 100f;//光線距離(照準判定の最大距離)
    public float CurrentDistance { get; private set; }// 交戦距離を外から参照できるようにする
    public float DamageForce { get; private set; }//威力減衰を含めたダメージを外から参照できるようにする

    float[] weapons_states;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerController.WeaponsNom == 0)//ハンドガン※選択武器によってステータスを変える
        {
            weapons_states = new float[6];
            {
                weapons_states[0] = 16;//最大ダメージ
                weapons_states[1] = 2;//最小ダメージ
                weapons_states[2] = 1f;//威力減衰
                weapons_states[3] = 0.75f;//射撃間隔
                weapons_states[4] = 10;//装弾数
                weapons_states[5] = 2;//再装填時間
                weapons_states[6] = 1f;//反動
            }
        }
        else if (PlayerController.WeaponsNom == 1)//サブマシンガン※選択武器によってステータスを変える
        {
            weapons_states = new float[7];
            {
                weapons_states[0] = 5;//最大ダメージ
                weapons_states[1] = 2;//最小ダメージ
                weapons_states[2] = 2f;//威力減衰
                weapons_states[3] = 0.1f;//射撃間隔
                weapons_states[4] = 25;//装弾数
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
                weapons_states[2] = 3f;//威力減衰
                weapons_states[3] = 1.5f;//射撃間隔
                weapons_states[4] = 6;//装弾数
                weapons_states[5] = 4;//再装填時間
                weapons_states[6] = 3f;//反動
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(CurrentDistance);//※相手プレイヤータグとの距離が正確化か測るため
    }

    public void Abc()//独自の関数
    {

    }

    public bool TryGiveDamage(GameObject owner)
    {
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Transform root = hit.collider.transform.root;

            if (!root.CompareTag("Player"))
                return false;

            if (root.gameObject == owner)
                return false;

            float damage = CalcDamage(hit.distance);

            //Damageable dmg = root.GetComponent<Damageable>();
            HpRodController dmg = root.GetComponentInChildren<HpRodController>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
                return true;
            }
        }
        return false;
    }

    float CalcDamage(float distance)
    {
        float maxDamage = weapons_states[0];
        float minDamage = weapons_states[1];
        float attenuation = weapons_states[2];

        float d = Mathf.Clamp01(distance / rayDistance);
        float damage = Mathf.Floor(Mathf.Lerp(maxDamage, minDamage, d * attenuation));//小数点は切り捨て※0になる可能性…こことは別の場所で何とかする
        //float damage = Mathf.Lerp(maxDamage, minDamage, d * attenuation);
        
        DamageForce = damage;//威力減衰を含めたダメージを外から参照できるようにする
        return damage;
    }

    public bool IsPlayerGiveDamage()
    {
        return true;
    }

}
