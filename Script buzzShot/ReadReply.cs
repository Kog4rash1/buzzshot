using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ReadReply : MonoBehaviour
{
    private string[] replyArray;

    //初回にファイルを読み込ませる
    // Use this for initialization
    void Start()
    {
        StartCoroutine(ReadFile());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(replyArray[0]);
        //Debug.Log(replyArray[1]);
        //Debug.Log(replyArray[2]);
    }

    /// <summary>
    /// 読み込み用関数
    /// </summary>
    IEnumerator ReadFile()
    {
        //ReplyStoreを読み込ませる
        string filepath = Application.dataPath + "/StreamingAssets/Reply/ReplyStore.txt";
        if (filepath.Contains("://") || filepath.Contains("://"))
        {
            WWW www = new WWW(filepath);
            yield return www;
            string reply = www.text;
            replyArray = reply.Split(new string[] { "\n\r" }, StringSplitOptions.None);
        }
        else
        {
            //ReadAllLinesで改行ごとに配列の中身を作ってくれる
            replyArray = File.ReadAllLines(filepath);
        }
    }

    /// <summary>
    /// リプライ配列の取得メソッド
    /// </summary>
    /// <returns></returns>
    public string[] GetArray()
    {
        return replyArray;
    }
}
