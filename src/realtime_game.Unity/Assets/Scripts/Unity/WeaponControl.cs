using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    //スクリプト
    [SerializeField] PlayerControl playerControl;

    //武器
    [SerializeField] private GameObject[] weapons; // 0:P320, 1:EP8, 2:M4
    [SerializeField] private GameObject WeaponsAll;//GameObject型の変数を宣言※ゲームオブジェクトをアタッチ
    [SerializeField] private GameObject WeaponM4;
    [SerializeField] private GameObject WeaponEP8;
    [SerializeField] private GameObject WeaponP320;

    public int WeaponsNom = 1;//武器の識別番号/public static int WeaponsNom

    bool isCountShake = false;//武器の揺れ
    int countShake = 0;//武器の揺れカウント

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var w in weapons)w.SetActive(false);

        // 選択武器だけ表示
        int index = WeaponsNom - 1;
        if (index >= 0 && index < weapons.Length)
            weapons[index].SetActive(true);

        /*if (WeaponsNom == 0)
        {
            WeaponsAll.SetActive(true);
            WeaponP320.SetActive(false);
            WeaponEP8.SetActive(false);
            WeaponM4.SetActive(false);
        }
         if (WeaponsNom == 1)
        {
            WeaponsAll.SetActive(true);
            WeaponP320.SetActive(true);
            WeaponEP8.SetActive(false);
            WeaponM4.SetActive(false);
        }
        else if (WeaponsNom == 2)
        {
            WeaponsAll.SetActive(true);
            WeaponP320.SetActive(false);
            WeaponEP8.SetActive(true);
            WeaponM4.SetActive(false);
        }
        else if (WeaponsNom == 3)
        {
            WeaponsAll.SetActive(true);
            WeaponP320.SetActive(false);
            WeaponEP8.SetActive(false);
            WeaponM4.SetActive(true);
        }*/

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Abc()//独自の関数
    {

    }

}
