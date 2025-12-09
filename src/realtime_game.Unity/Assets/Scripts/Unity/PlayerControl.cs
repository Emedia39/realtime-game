using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rb;//リジットボディ
    private float moveSpeed = 5.0f;//プレイヤー移動速度
    //private float cameraSpeed = 5.0f;//カメラの視点速度
    private float jumpPower = 5.0f;//ジャンプ上昇速度
    private bool isJump = false;//

    // FPS視点パラメータ
    public Transform neck;                  // プレイヤーの首のTransformを指定
    public float sensitivity = 5.0f;     // マウス感度（視点の移動の速さを調整）
    public float minVertical = -90.0f;   // 視点の最小角度（縦の回転制限）
    public float maxVertical = 90.0f;    // 視点の最大角度（縦の回転制限）
    // 演算用変数
    private float rotationX = 0f;       // 縦方向の回転角度（首の回転）
    
    /*public enum DIRECTION_TYPE//列挙型#方向の種類
    {
        STOP,
        RIGHT,
        LEFT
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // カーソルを非表示＆ロック
        Cursor.lockState = CursorLockMode.Locked;   // カーソルを画面中央に固定
        Cursor.visible = false;                     // カーソルを非表示にする

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()//Rigidbodyを使った移動の処理はFixedUpdateを使う
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) dir += transform.forward;
        if (Input.GetKey(KeyCode.S)) dir -= transform.forward;
        if (Input.GetKey(KeyCode.D)) dir += transform.right;
        if (Input.GetKey(KeyCode.A)) dir -= transform.right;

        // 入力がないとき normalized すると危険なので条件分岐する
        Vector3 move = Vector3.zero;
        if (dir != Vector3.zero)move = dir.normalized * moveSpeed;

        // y だけは維持
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);//velocity
        
        /*
        switch (direction)
        {
            case DIRECTION_TYPE.STOP:
                speed = 0;
                break;

            case DIRECTION_TYPE.RIGHT:
                speed = 3;
                transform.localScale = new Vector3(1, 1, 1);
                break;

            case DIRECTION_TYPE.LEFT:
                speed = -3;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);//速度の変更
        */
    }

    void Update()
    {
        //マウスでカメラの視点を操作する
        // マウス入力の取得
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;  // 横のマウス移動量を取得し、感度で調整
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;  // 縦のマウス移動量を取得し、感度で調整

        // Player（体）の回転（左右）
        transform.Rotate(0, mouseX, 0);   // プレイヤー（体）の左右の回転をマウスX方向の入力に合わせて行う

        // Neck（首）の回転（上下）
        rotationX -= mouseY; // マウスY方向の入力によって縦方向の回転を更新
        rotationX = Mathf.Clamp(rotationX, minVertical, maxVertical);   // 回転角度を指定された範囲に制限
        neck.localRotation = Quaternion.Euler(rotationX, 0, 0);
        /*float x = Input.GetAxis("Mouse X") * cameraSpeed;
        //if(Input.GetMouseButton(0))
        {
            if(Mathf.Abs(x) > 0.1f)
            {
                transform.RotateAround(transform.position, Vector3.up, x);
            }
        }
        //マウスでカメラの視点を操作する
        float y = Input.GetAxis("Mouse Y") * cameraSpeed;
        //if(Input.GetMouseButton(0))
        {
            if (Mathf.Abs(y) > 0.1f)
            {
                transform.RotateAround(transform.position, Vector3.right, y);
            }
        }*/

        //スペースキーを押すとジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            isJump = true;
        }

        //Debug.Log(transform.up);

        /*float x = Input.GetAxis("Horizontal");//横方向キーの入力を取得/wasd
        //animator.SetFloat("Speed", Mathf.Abs(x)); //絶対値※Mathf

        if (x == 0)
        {
            direction = DIRECTION_TYPE.STOP;//止まっている
        }
        else if (x > 0)
        {
            direction = DIRECTION_TYPE.RIGHT;//右へ
        }
        else if (x < 0)
        {
            direction = DIRECTION_TYPE.LEFT;//左へ
        }*/

    }

    //Ground着地したらisJumpをfalseに
    private void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJump = false;
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;
        }
    }
    */
}
