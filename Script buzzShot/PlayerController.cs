using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

//このスクリプトはPlayerオブジェクトに使用

public class PlayerController : MonoBehaviour
{
    Rigidbody rigidbody;
    [SerializeField]
    private float rigidSpeed = 10000;

    [SerializeField]
    private float speed = 0.05f;

    [SerializeField]
    private float rotateSpeed;

    //SetActive用にカメラをゲームオブジェクトで取得
    [SerializeField]
    private GameObject fpsCam;
    [SerializeField]
    private GameObject mainCam;
    [SerializeField]
    private GameObject tpsCam;
    [SerializeField]
    private GameObject tpsCamParent;

    //rotationの軸の変更
    [SerializeField]
    public static bool rotationVerticalConfig;
    [SerializeField]
    public static bool rotationHorizonConfig;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private GameObject charactor;

    private Transform mainCamTrans;
    private Vector3 camforward;
    private Vector3 ido;
    private Vector3 Animdir = Vector3.zero;

    private bool mainCamActive;

    private bool nowChangeCamera;

    private float storeSpeed;

    private Vector3 startTpsPosition;

    private AccountStruct accountStruct;
    private Vector3 charaDefaltPos;

    // Use this for initialization
    void Start()
    {
        startTpsPosition = tpsCamParent.transform.localPosition;
        storeSpeed = speed;
        rigidbody = GetComponent<Rigidbody>();
        mainCamTrans = Camera.main.transform;
        mainCamActive = true;
        nowChangeCamera = false;
        charaDefaltPos = charactor.transform.localPosition;
        ChangeCamera();
    }

    void Update()
    {
        //Lボタンを押す
        if (Input.GetButtonDown("CameraChange"))
        {
            Debug.Log("カメラ交代");
            ChangeCamera();
        }
    }

    private IEnumerator CameraChangeMove()
    {
        if(nowChangeCamera)
        {
            yield break;
        }
        if(mainCamActive)
        {
            ChangeCamera();
            yield break;
        }
        nowChangeCamera = true;
        //bool cameraMatch = true;
        Debug.Log("移動化開始");
        Vector3 storePosition = tpsCamParent.transform.position;
        float distance = Vector3.Distance(tpsCamParent.transform.position, mainCamTrans.position);
        while (distance >= 0.3f)
        {
            distance = Vector3.Distance(tpsCamParent.transform.position, mainCamTrans.position);

            //プレイヤーの座標まで、追いつかせる
            Vector3 velocity = mainCamTrans.position - tpsCamParent.transform.position;
            if (velocity.sqrMagnitude != 0) velocity.Normalize();
            tpsCamParent.transform.position += velocity.normalized * 7.0f * Time.deltaTime;
            distance = Vector3.Distance(tpsCamParent.transform.position, mainCamTrans.position);
            Debug.Log("Dis" + distance);
            Debug.Log("Vel" + velocity);
            //if (distance <= 0.1f) cameraMatch = false;

            yield return new WaitForEndOfFrame();
        }

        nowChangeCamera = false;

        Debug.Log("移動完了");
        ChangeCamera();

        tpsCamParent.transform.position = storePosition;
    }

    public bool IsCameraChange()
    {
        return nowChangeCamera;
    }

    /// <summary>
    /// カメラ変更用判定メソッド
    /// </summary>
    void ChangeCamera()
    {
        Vector3 tpsRotate = Vector3.zero;
        if (mainCamActive)
        {
            charactor.transform.localPosition = charaDefaltPos;
            charactor.transform.localEulerAngles = Vector3.zero;
            fpsCam.transform.localEulerAngles = tpsCamParent.transform.localEulerAngles;
            tpsCamParent.transform.localPosition = startTpsPosition;
            tpsCamParent.transform.localEulerAngles = Vector3.zero;
            mainCam.SetActive(false);
            tpsCam.SetActive(true);
            mainCamActive = false;
            Debug.Log("非アクティブ");
        }
        else
        {
            charactor.transform.localPosition = charaDefaltPos;
            transform.localEulerAngles = new Vector3(0, tpsCamParent.transform.localEulerAngles.y + transform.localEulerAngles.y, 0);
            tpsCamParent.transform.localPosition = startTpsPosition;
            mainCam.SetActive(true);
            tpsCam.SetActive(false);
            mainCamActive = true;
            Debug.Log("アクティブ");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (nowChangeCamera) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(horizontal), Mathf.Abs(vertical)));

        //Animating(horizontal, vertical);

        //カメラのTransformが取得されてれば実行
        if (mainCamTrans != null)
        {
            //2つのベクトルの各成分の乗算(Vector3.Scale)。単位ベクトル化(.normalized)
            camforward = Vector3.Scale(tpsCamParent.transform.forward, new Vector3(1, 0, 1)).normalized;

            if (mainCamActive)
            {
                ido = vertical * mainCam.transform.forward * speed + horizontal * mainCam.transform.right * speed;
            }
            else
            {
                //移動ベクトルをidoというトランスフォームに代入
                ido = vertical * camforward * speed + horizontal * tpsCamParent.transform.right * speed;
                //Debug.Log("Forward:" + camforward + ",Horizon:" + tpsCamParent.transform.right);
                if (horizontal != 0 || vertical != 0)
                {
                    Vector3 rotatePlayer = camforward * vertical + tpsCamParent.transform.right * horizontal;
                    //Debug.Log("Player回転:" + rotatePlayer);
                    charactor.transform.rotation = Quaternion.LookRotation(rotatePlayer);
                }
            }
        }

        //現在のポジションにidoのトランスフォームの数値を入れる
        //transform.position = new Vector3(transform.position.x + ido.x, transform.position.y, transform.position.z + ido.z);
        rigidbody.velocity = new Vector3(ido.x, 0,ido.z) * rigidSpeed*Time.deltaTime;

        //回転処理
        float rotationX = Input.GetAxis("RotationX");
        float rotationY = Input.GetAxis("RotationY");
        //向ける角度を変えたい場合ここを変更する
        Vector2 rotationLimit = new Vector2(300, 0);


        if (!mainCamActive)
        {
            //角度の取得はtransform.localEulerAngles
            float x = tpsCamParent.transform.localEulerAngles.x;

            switch (rotationVerticalConfig)
            {
                case false:
                    //falseの時、反転無し
                    //縦回転はローカル軸
                    //範囲を制限
                    tpsCamParent.transform.RotateAround(tpsCamParent.transform.position, tpsCamParent.transform.right, rotationY * rotateSpeed);
                    if (tpsCamParent.transform.localEulerAngles.x < rotationLimit.x && tpsCamParent.transform.localEulerAngles.x > 260)
                    {
                        x = rotationLimit.x;
                    }
                    else
                    {
                        x = (tpsCamParent.transform.localEulerAngles.x > rotationLimit.y && tpsCamParent.transform.localEulerAngles.x < 240) ? rotationLimit.y : tpsCamParent.transform.localEulerAngles.x;
                    }
                    tpsCamParent.transform.localEulerAngles = new Vector3(x, tpsCamParent.transform.localEulerAngles.y, tpsCamParent.transform.localEulerAngles.z);
                    break;

                case true:
                    //trueの時、反転
                    //縦回転はローカル軸
                    //範囲を制限
                    tpsCamParent.transform.RotateAround(tpsCamParent.transform.position, tpsCamParent.transform.right, -rotationY * rotateSpeed);
                    if (tpsCamParent.transform.localEulerAngles.x < rotationLimit.x && tpsCamParent.transform.localEulerAngles.x > 260)
                    {
                        x = rotationLimit.x;
                    }
                    else
                    {
                        x = (tpsCamParent.transform.localEulerAngles.x > rotationLimit.y && tpsCamParent.transform.localEulerAngles.x < 240) ? rotationLimit.y : tpsCamParent.transform.localEulerAngles.x;
                    }
                    tpsCamParent.transform.localEulerAngles = new Vector3(x, tpsCamParent.transform.localEulerAngles.y, tpsCamParent.transform.localEulerAngles.z);
                    break;
            }

            switch (rotationHorizonConfig)
            {
                case false:
                    //横回転はグローバル軸
                    //tpsカメラの親オブジェクトを回転
                    transform.RotateAround(transform.position, Vector3.up, rotationX * rotateSpeed);
                    break;

                case true:
                    //横回転はグローバル軸
                    //tpsカメラの親オブジェクトを回転
                    transform.RotateAround(transform.position, Vector3.up, -rotationX * rotateSpeed);
                    break;
            }
        }
        else
        {
            //角度の取得はtransform.localEulerAngles
            float x = fpsCam.transform.localEulerAngles.x;

            switch (rotationVerticalConfig)
            {
                case false:
                    //falseの時、反転無し
                    //縦回転はローカル軸
                    //範囲を制限
                    fpsCam.transform.RotateAround(fpsCam.transform.position, fpsCam.transform.right, rotationY * rotateSpeed);
                    if (fpsCam.transform.localEulerAngles.x < rotationLimit.x && fpsCam.transform.localEulerAngles.x > 260)
                    {
                        x = rotationLimit.x;
                    }
                    else
                    {
                        x = (fpsCam.transform.localEulerAngles.x > rotationLimit.y + 45 && fpsCam.transform.localEulerAngles.x < 240) ? rotationLimit.y + 45 : fpsCam.transform.localEulerAngles.x;
                    }
                    fpsCam.transform.localEulerAngles = new Vector3(x, fpsCam.transform.localEulerAngles.y, fpsCam.transform.localEulerAngles.z);
                    break;

                case true:
                    //trueの時、反転
                    //縦回転はローカル軸
                    //範囲を制限
                    fpsCam.transform.RotateAround(fpsCam.transform.position, fpsCam.transform.right, -rotationY * rotateSpeed);
                    if (fpsCam.transform.localEulerAngles.x < rotationLimit.x && fpsCam.transform.localEulerAngles.x > 260)
                    {
                        x = rotationLimit.x;
                    }
                    else
                    {
                        x = (fpsCam.transform.localEulerAngles.x > rotationLimit.y + 45 && fpsCam.transform.localEulerAngles.x < 240) ? rotationLimit.y + 45 : fpsCam.transform.localEulerAngles.x;
                    }
                    fpsCam.transform.localEulerAngles = new Vector3(x, fpsCam.transform.localEulerAngles.y, fpsCam.transform.localEulerAngles.z);
                    break;
            }

            //Debug.Log(transform.localEulerAngles + "ローテーション");
            switch (rotationHorizonConfig)
            {
                case false:
                    //横回転はグローバル軸
                    transform.RotateAround(transform.position, Vector3.up, rotationX * rotateSpeed);
                    break;

                case true:
                    //横回転はグローバル軸
                    transform.RotateAround(transform.position, Vector3.up, -rotationX * rotateSpeed);
                    break;
            }
        }
        //少しずれるため位置の固定
        charactor.transform.localPosition -= new Vector3(charactor.transform.localPosition.x, 0, charactor.transform.localPosition.z) * Time.deltaTime;

    }

    void Animating(float h, float v)
    {
        bool walking = h != 0f || v != 0f;
        anim.SetBool("IsWalking", walking);
    }

    //private void OnDestroy()
    //{
    //    //プレイヤー情報の取得
    //    accountStruct = new AccountStruct(PhotoTexture2D.GetInstance.GetName(), PhotoTexture2D.GetInstance.GetFollower());
    //    //リストへのプレイヤーの挿入
    //    //エラー箇所
    //    AccountRankingManager.GetInstance().InsertList(accountStruct);
    //    //テキストへの書き込み
    //    AccountRankingManager.GetInstance().WriteFile();
    //}
}