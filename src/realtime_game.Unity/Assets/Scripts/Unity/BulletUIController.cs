using UnityEngine;
using UnityEngine.UI;
//using TMPro;//TMP‚È‚ç

public class BulletUIController : MonoBehaviour
{
    [SerializeField] WeaponController weaponController;
    [SerializeField] Text bulletText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bulletText.text =
        "rest : " +
        weaponController.nowBullets + " / " +
        weaponController.weapons_states[4];
    }

}
