using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UI非表示・表示切替用スクリプト
//使用する際はキャンバスに追加すること

//他のクラスでSerializeField使用のフィールドを扱えるようにする
[System.Serializable]
public class TextSwitchDisplay : MonoBehaviour
{
    [SerializeField]
    private bool active;

    //実質publicでフィールドを扱う
    [SerializeField]
    private GameObject gameObject;

    [SerializeField]
    private string InputName;

    // Use this for initialization
    void Start()
    {
        //active = false;
        gameObject.SetActive(active);
        if (InputName == null)
        {
            InputName = "UIActive";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //UIの出現、消去
        if (Input.GetButtonDown(InputName))
        {
            active = !active;
            gameObject.SetActive(active);
        }
    }
}
