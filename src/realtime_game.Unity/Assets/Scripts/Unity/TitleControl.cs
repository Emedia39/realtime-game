using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour
{
    //SE
    //[SerializeField] AudioClip gameStartSE;//クリアSE
    //AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadRoginScene()//ボタンでシーン移行
    {
        //audioSource.PlayOneShot(gameStartSE);

        //シーン遷移
        //SceneManager.LoadScene("GameScene");//シーン切り替え
        Initiate.Fade("RoginScene", Color.black, 1.0f);//移動先のシーン#色指定#フェードにかかる時間
    }

}
