using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    //スクリプト
    [SerializeField] PlayerControl playerControl;

    //照準の自動トリガー
    [SerializeField] private Camera fpsCamera;//カメラ取得
    [SerializeField] private float rayDistance = 100f;//光線距離(照準判定の最大距離)
    public float CurrentDistance { get; private set; }// 交戦距離を外から参照できるようにする

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PlayerControl.WeaponsNom == 0)//選択武器によってステータスを変える
        {

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

    public bool IsPlayerInReticle()
    {
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            // 距離を保存
            CurrentDistance = hit.distance;

            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        // 当たらなかったら無効値
        CurrentDistance = -1f;
        return false;
    }
    /*public bool IsPlayerInReticle()
    {
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }*/
    /*public bool IsPlayerInReticleArea()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        Vector3 screenPos =
            fpsCamera.WorldToScreenPoint(player.transform.position);

        // カメラの前にいない
        if (screenPos.z <= 0) return false;

        // 画面中心
        Vector2 screenCenter = new Vector2(
            Screen.width / 2f,
            Screen.height / 2f
        );

        // 中心との距離
        float distance =
            Vector2.Distance(screenCenter, screenPos);

        return distance <= radiusPixel;
    }*/
    /*public bool IsPlayerInArea()//
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        Vector3 viewportPos =
            targetCamera.WorldToViewportPoint(player.transform.position);

        // カメラの前にいるか
        if (viewportPos.z <= 0) return false;

        // 特定範囲内か
        if (viewportPos.x >= minX && viewportPos.x <= maxX &&
            viewportPos.y >= minY && viewportPos.y <= maxY)
        {
            return true;
        }

        return false;
    }*/

}
