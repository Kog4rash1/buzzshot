using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

//アカウントランキング管理(シングルトン)
//継承禁止
public sealed class AccountRankingManager
// : MonoBehaviour
//これを継承するとnewするとエラーを吐く
//つまりシングルトン(インスタンス取得)に向いていない
//MonoBehaviourを利用する場合
//Instaniate(Resouces)でResoucesフォルダでprefabを生成DontDestroyOnLoad
{
    #region フィールド

    private static AccountRankingManager accountRankingManager;

    //改行用配列
    private string[] arrayLine;
    //コンマ用配列
    private string[][] arrayComma;
    //構造体用配列
    private List<AccountStruct> listStruct;

    #endregion

    /// <summary>
    /// リストの取得プロパティ
    /// </summary>
    public List<AccountStruct> GetList
    {
        get { return listStruct; }
    }

    /// <summary>
    /// インスタンスの取得
    /// </summary>
    /// <returns></returns>
    public static AccountRankingManager GetInstance()
    {
        if (accountRankingManager == null) accountRankingManager = new AccountRankingManager();
        return accountRankingManager;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AccountRankingManager()
    {
        //StartCoroutine( );
        ReadFile();
        //配列の実体生成
        arrayComma = new string[10][];
        listStruct = new List<AccountStruct>();
        int j = 0;

        //エラー箇所(Out of Range)※修正完了
        for (int i = 0; i < arrayLine.Length; i++)
        {
            //arrayLineの要素数がarrayCommaの要素数を超えていた場合
            //強制終了
            if (!(i < arrayComma.Length))
            {
                break;
            }

            while (true)
            {
                if (arrayLine[j].Contains(","))
                {
                    //コンマごとに配列の中身挿入
                    arrayComma[i] = arrayLine[j].Split(',');
                    break;
                }
                else
                {
                    j += 1;
                }
            }

            j++;
        }

        //エラー箇所(Out of Range)※修正完了
        for (int i = 0; i < arrayComma.Length; i++)
        {
            //構造体配列の中身挿入
            //arrayComma[i]の要素数が間違って2を超えていても読み込まないように定数挿入
            listStruct.Add(new AccountStruct(arrayComma[i][0], int.Parse(arrayComma[i][1])));
        }

        //フォロワーの数で降順ソート
        listStruct.Sort((a, b) => b.Follower - a.Follower);

        //もし10より要素数があった場合
        if (listStruct.Count > 10)
        {
            for (int i = 10; i < listStruct.Count; i++)
            {
                listStruct.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// プレイヤー情報をリストに挿入する
    /// </summary>
    /// <param name="accountStruct">Playerのアカウント</param>
    public void InsertList(AccountStruct playerAccount)
    {
        for (int i = 0; i < listStruct.Count; i++)
        {
            //プレイヤーのフォロワー数がリスト内のフォロワー数より多かった場合その位置に挿入
            if (playerAccount.Follower > listStruct[i].Follower)
            {
                listStruct.Insert(i, playerAccount);
                break;
            }
        }

        //もし10より要素数があった場合
        if (listStruct.Count > 10)
        {
            for (int i = 10; i < listStruct.Count; i++)
            {
                listStruct.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 書き込み用関数
    /// </summary>
    public void WriteFile()
    {
        //セーブ先のファイルパス
        string filepath = Application.dataPath + "/StreamingAssets/AccountRanking/AccountSave.txt";

        string[] structArray = new string[listStruct.Count];

        for (int i = 0; i < listStruct.Count; i++)
        {
            structArray[i] = listStruct[i].ToString();
        }
        
        File.WriteAllLines(filepath, structArray, Encoding.GetEncoding(932));
    }

    /// <summary>
    /// 読み込み用関数
    /// </summary>
    void ReadFile()
    {
        //AccountSaveを読み込ませる
        string filepath = Application.dataPath + "/StreamingAssets/AccountRanking/AccountSave.txt";
        if (filepath.Contains("://") || filepath.Contains("://"))
        {
            WWW www = new WWW(filepath);
            //yield return www;
            string reply = www.text;
            arrayLine = reply.Split(new string[] { "\n\r" }, StringSplitOptions.None);
        }
        else
        {
            //ReadAllLinesで改行ごとに配列の中身を作ってくれる
            arrayLine = File.ReadAllLines(filepath, Encoding.GetEncoding(932));
        }
    }
}