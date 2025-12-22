using UnityEngine;
using UnityEngine.UI;

public class HpRodController : MonoBehaviour
{
    public float maxHP;
    public float currentHP = 100;

    public Image hpBarImage; // HPバー
    public Text Hp;          // HP数値

    //public Color blueColor = Color.cyan;//new Color(37f, 182f, 252f, 255f);
    //public Color yellowColor = Color.yellow;//new Color(241f, 187f, 46f, 255f);
    //public Color redColor = Color.red;// new Color(255f, 79f, 65f, 255f);

    public Color blueColor = new Color(37f, 182f, 252f, 255f);//= Color.cyan;
    public Color yellowColor = new Color(241f, 187f, 46f, 255f);//= Color.yellow;
    public Color redColor = new Color(255f, 79f, 65f, 255f);// = Color.red;

    private RectTransform barRT;
    private float maxBarWidth;

    void Start()
    {
        maxHP = 100;
        currentHP = 100;//

        barRT = hpBarImage.GetComponent<RectTransform>();
        maxBarWidth = barRT.rect.width;
        UpdateHPUI();
    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        // テキスト
        Hp.text = currentHP.ToString();

        // バー幅
        float rate = (float)currentHP / maxHP;
        barRT.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            maxBarWidth * rate
        );

        // 色切り替え
        if (currentHP > 50)
            hpBarImage.color = blueColor;
        else if (currentHP > 20)
            hpBarImage.color = yellowColor;
        else
            hpBarImage.color = redColor;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} に {damage} ダメージ");

        if (currentHP <= 0)
        {
            KnockDown();
        }
    }

    void KnockDown()
    {
        Debug.Log($"{gameObject.name} は倒された");
        gameObject.SetActive(false);
    }

}