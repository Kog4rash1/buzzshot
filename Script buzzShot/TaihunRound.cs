using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回転しながら上昇加工するメソッド
/// </summary>
public class TaihunRound : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigid;

    [SerializeField]
    private float roundSpeed;

    [SerializeField]
    private float upSpeed;

    [SerializeField]
    private GameObject parent;

    private Rigidbody parentRigidbody;

    //上昇値の合計
    private float total;
    //上昇中か?
    private bool rising;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //回転
        rigid.angularVelocity = new Vector3(0, roundSpeed, 0);

        //上昇中なら
        if (rising == true)
        {
            if (parent != null)
            {
                rigid.velocity = new Vector3(0, upSpeed, 0) + parentRigidbody.velocity;
            }
            else
            {
                rigid.velocity = new Vector3(0, upSpeed, 0);
            }

            total += upSpeed;

            if (total >= 50)
            {
                rising = false;
            }
        }
        else
        //下降中なら
        {
            if (parent != null)
            {
                rigid.velocity = new Vector3(0, -upSpeed, 0) + parentRigidbody.velocity;
            }
            else
            {
                rigid.velocity = new Vector3(0, -upSpeed, 0);
            }

            total -= upSpeed;

            if (total <= 0)
            {
                rising = true;
            }
        }
    }
    public void Init()
    {
        rigid = GetComponent<Rigidbody>();
        rising = true;
        total = 0;
        if (parent != null)
        {
            parentRigidbody = parent.GetComponent<Rigidbody>();
        }
    }

    public void SetParent(GameObject parent)
    {
        this.parent = parent;
        parentRigidbody = parent.GetComponent<Rigidbody>();
    }
}
