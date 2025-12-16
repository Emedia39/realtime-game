using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //SE
    //[SerializeField] AudioClip gameStartSE;//クリアSE
    //AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // カーソルを非表示＆ロック
        Cursor.lockState = CursorLockMode.Locked;//カーソルを画面中央に固定
        Cursor.visible = false;//カーソル非表示
        //Cursor.lockState = CursorLockMode.None;//カーソルを自由に
        //Cursor.visible = true;//カーソル表示

        //audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadResultScene()//ボタンでシーン移行
    {
        //audioSource.PlayOneShot(gameStartSE);

        //シーン遷移
        //SceneManager.LoadScene("GameScene");//シーン切り替え
        Initiate.Fade("ResultScene", Color.black, 1.0f);//移動先のシーン#色指定#フェードにかかる時間
    }
}
