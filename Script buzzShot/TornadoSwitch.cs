using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSwitch : MonoBehaviour
{
    //アクティブ状態の変化
    [SerializeField]
    private bool activeSwitch;

    //ターゲットオブジェクト
    [SerializeField]
    private GameObject target;

    //保存用
    private Vector3 position;

    // Use this for initialization
    void Start()
    {
        target.SetActive(activeSwitch);
        position = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //確率で出現させたり消滅させたりする
        int num = Random.Range(0, 500);
        Debug.Log(num);

        if (num == 0)
        {
            target.transform.position = position;
            activeSwitch = !activeSwitch;
            Debug.Log("Tornado");
        }

        //表示状態の切り替え
        target.SetActive(activeSwitch);
    }
}
