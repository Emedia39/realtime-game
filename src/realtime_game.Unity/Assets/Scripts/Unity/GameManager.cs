using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //ポーズのUI
    [SerializeField] Image PoseImage;
    [SerializeField] Text PoseText;
    [SerializeField] GameObject PoseButton;
    public bool pausingDisplay { get; private set; } = false;//ポーズ中かどうか

    //SE
    //[SerializeField] AudioClip gameStartSE;//クリアSE
    //AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ポーズUI非表示
        PoseImage.enabled = false;
        PoseText.enabled = false;
        PoseButton.SetActive(false);

        // カーソルをロック＆非表示
        Cursor.lockState = CursorLockMode.Locked;//カーソルを画面中央に固定
        Cursor.visible = false;//カーソル非表示

        //audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {//ESCキーを押した場合

            if (pausingDisplay == true)
            {
                pausingDisplay = false;

                PoseImage.enabled = false;
                PoseText.enabled = false;
                PoseButton.SetActive(false);
                //PoseButton.enabled = false;

                // カーソルをロック＆非表示
                Cursor.lockState = CursorLockMode.Locked;//カーソルを画面中央に固定
                Cursor.visible = false;//カーソル非表示

            }
            else if (pausingDisplay == false)
            {
                pausingDisplay = true;

                PoseImage.enabled = true;
                PoseText.enabled = true;
                PoseButton.SetActive(true);
                //PoseButton.enabled = true;

                // カーソルをフリー＆表示
                Cursor.lockState = CursorLockMode.None;//カーソルを自由に
                Cursor.visible = true;//カーソル表示

            }
        }

            /*if (Input.GetKeyDown(KeyCode.Escape))
            {//ESCキーを押した場合
    #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームを強制終了
    #else//ビルドの場合
            Application.Quit();
    #endif
            }*/
        }

    public void LoadResultScene()//ボタンでシーン移行
    {
        //audioSource.PlayOneShot(gameStartSE);

        //シーン遷移
        //SceneManager.LoadScene("GameScene");//シーン切り替え
        Initiate.Fade("ResultScene", Color.black, 1.0f);//移動先のシーン#色指定#フェードにかかる時間
    }

    public void EndUnity()//ボタンでシーン移行
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームを強制終了
#else//ビルドの場合
            Application.Quit();
#endif
    }

}
