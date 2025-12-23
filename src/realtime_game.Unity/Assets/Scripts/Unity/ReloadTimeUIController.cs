using UnityEngine;
using UnityEngine.UI;

public class ReloadTimeUIController : MonoBehaviour
{
    [SerializeField] WeaponController weaponController;
    [SerializeField] Image[] reloadtimeImage;

    public float maxReloadTime;
    public float currenReloadTime = 100;

    public Image ReloadTimeBarImage; //クールタイムバー
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
        //gameObject.SetActive(true);

        maxReloadTime = 0;//0
        currenReloadTime = 0;//0

        foreach (Image Reload in reloadtimeImage)
        {
            Reload.enabled = false;
        }

        barRT = ReloadTimeBarImage.GetComponent<RectTransform>();
        maxBarWidth = barRT.rect.width;
    }

    private void Update()
    {
        if (weaponController.restReloadTime >= 0)//リロード完了秒数まで0.0+以上
        {
            foreach (Image cool in reloadtimeImage)
            {
                cool.enabled = true;
            }

            maxReloadTime = weaponController.weapons_states[5];//仮

            currenReloadTime = weaponController.restReloadTime;
            //Debug.Log(currenReloadTime);

            // バー幅
            float rate = (float)currenReloadTime / maxReloadTime;//currenReloadTime
            barRT.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                maxBarWidth * rate
            );//ここら辺が問題？

        }
        else//リロード完了秒数まで0以下
        {
            foreach (Image Reload in reloadtimeImage)
            {
                Reload.enabled = false;
            }
        }

    }

}
