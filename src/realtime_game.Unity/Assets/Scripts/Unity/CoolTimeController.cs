using UnityEngine;
using UnityEngine.UI;

public class CoolTimeController : MonoBehaviour
{
    [SerializeField] WeaponController weaponController;
    [SerializeField] Image[] cooltimeImage;

    public float maxCoolTime;
    public float currenCoolTime = 100;

    public GameObject gameObject;
    public Image coolTimeBarImage; //クールタイムバー
    //public Text Hp;          // HP数値
    
    //public Color blueColor = Color.cyan;//new Color(37f, 182f, 252f, 255f);
    //public Color yellowColor = Color.yellow;//new Color(241f, 187f, 46f, 255f);
    //public Color redColor = Color.red;// new Color(255f, 79f, 65f, 255f);

    /*public Color blueColor = new Color(37f, 182f, 252f, 255f);//= Color.cyan;
    public Color yellowColor = new Color(241f, 187f, 46f, 255f);//= Color.yellow;
    public Color redColor = new Color(255f, 79f, 65f, 255f);// = Color.red;*/

    private RectTransform barRT;
    private float maxBarWidth;

    void Start()
    {
        maxCoolTime =0;//0
        currenCoolTime = 0;//0

        foreach (Image cool in cooltimeImage)
        {
            cool.enabled = false;
        }

        barRT = coolTimeBarImage.GetComponent<RectTransform>();
        maxBarWidth = barRT.rect.width;
    }

    private void Update()
    {
        if (weaponController.restFireTime >= 0)//射撃可能秒数まで0.0+以上
        {
            foreach (Image cool in cooltimeImage)
            {
                cool.enabled = true;
            }

            gameObject.SetActive(true);

            maxCoolTime = weaponController.weapons_states[3];//仮

            currenCoolTime = weaponController.restFireTime;
            //Debug.Log(currenCoolTime);
            Debug.Log(weaponController.weapons_states[3]);

            // バー幅
            float rate = (float)currenCoolTime / maxCoolTime;//currenCoolTime
            barRT.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                maxBarWidth * rate
            );

        }
        else//射撃可能秒数まで0以下
        {
            foreach (Image cool in cooltimeImage)
            {
                cool.enabled = false;
            }
        }

        //Debug.Log(barRT);
    }

}
