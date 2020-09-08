using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubjectRanking : MonoBehaviour
{
    #region フィールド

    //ミニマップ
    [SerializeField]
    private MiniMap mini;
    //ミニトレンド
    [SerializeField]
    private MiniTrend miniTrend;

    //被写体管理
    [SerializeField]
    private SubjectManager manager;
    //トレンドのコンプリート用オブジェクト
    [SerializeField]
    private TrendComplete complete;
    //トレンドがアップデートされたことを示すテキスト
    [SerializeField]
    private GameObject updateText;

    //処理を行う時間
    [SerializeField]
    private float activeTime;

    //前回のUpdateからの経過時間
    private float progressTime;

    //存在するオブジェクトのリスト
    private List<Subject> activeSubjectList;

    //MiniMap取得用ランキングリスト
    private List<SubjectEnum> subjectRankingList;

    //被写体のテキスト管理用配列
    [SerializeField]
    private Text[] subjectTextObjectList = new Text[3];

    //被写体の画像管理用配列
    [SerializeField]
    private Image[] subjectImageObjectList = new Image[3];

    //ランキングに入っている被写体をとった場合隠すためのイメージ
    [SerializeField]
    private Image[] rankingHideImages;

    //テキスト変更用
    private Dictionary<SubjectEnum, string> textChanger;

    //switch文用
    private SubjectEnum subjectEnum;

    //画像変更用
    private Dictionary<SubjectEnum, string> imageAddressDict;
    private Dictionary<string, Sprite> imageChanger;

    //subjectEnum用
    private List<SubjectEnum> subjectList;

    //時間を描画してくれる
    [SerializeField]
    private TimerDraw trendTimerText;

    #endregion

    //テキストの受け渡し
    public string GetText(SubjectEnum type)
    {
        return textChanger[type];
    }

    // Use this for initialization
    void Awake()
    {
        //各変数初期化
        textChanger = new Dictionary<SubjectEnum, string>();
        imageAddressDict = new Dictionary<SubjectEnum, string>();
        imageChanger = new Dictionary<string, Sprite>();
        activeSubjectList = new List<Subject>();
        subjectList = new List<SubjectEnum>();
        subjectRankingList = new List<SubjectEnum>();

        //全被写体リストの作成
        for (int i = 0; i < (int)SubjectEnum.END; i++)
        {
            subjectList.Add((SubjectEnum)i);
        }

        //イメージ画像の設定
        SetImage();
        //テキストの設定
        SetText();
        //初期化
        Init();
        updateText.SetActive(false);
    }

    /// <summary>
    ///変数subjectRankingの中身を返すメソッド
    ///番号の中身を返す(0番から)
    /// </summary>
    /// <param name="rankNum">配列の番号</param>
    /// <returns></returns>
    public SubjectEnum GetRank(int rankNum)
    {
        Debug.Log(subjectRankingList.Count + "要素");
        return subjectRankingList[rankNum];
    }

    // Update is called once per frame
    void Update()
    {
        progressTime += Time.deltaTime;

        //時間が来たら
        if (progressTime >= activeTime)
        {
            Debug.Log("入った");
            Init();
            StartCoroutine(Timer());
        }
    }

    public void Init()
    {
        RaningChangeTop();
        manager.Init();
        mini.MapRanking(this);
        miniTrend.SetTrend(this);
        progressTime = 0;
        trendTimerText.Init();
        for (int i = 0; i < rankingHideImages.Length; i++)
        {
            rankingHideImages[i].color = new Color(0, 0, 0, 0.0f);
        }
    }

    public void TrendIsShot()
    {
        int count = 0;

        for (int i = 0; i < subjectRankingList.Count; i++)
        {
            for (int j = 0; j < manager.GetIsShotSubject().Length; j++)
            {
                if (subjectRankingList[i] == manager.GetIsShotSubject()[j])
                {
                    rankingHideImages[i].color = new Color(0, 0, 0, 0.6f);
                    miniTrend.MiniTrendHide(i);
                    count++;
                }
            }
        }

        if (count >= subjectRankingList.Count)
        {
            Init();
            StartCoroutine(Timer());
            StartCoroutine(complete.BonusMove());
        }
    }

    public void RaningChangeTop()
    {
        SubjectEnum[] subs = manager.RankingSet(subjectRankingList.ToArray());
        subjectRankingList.Clear();
        activeSubjectList.Clear();
        for (int i = 0; i < subjectTextObjectList.Length; i++)
        {
            //現在のキーがtextChangerとimageAddressDictに存在するか？
            if (textChanger.ContainsKey(subs[i]) && imageAddressDict.ContainsKey(subs[i]))
            {
                //テキストの変更
                subjectTextObjectList[i].text = textChanger[subs[i]];
                //イメージの中身の変更
                Debug.Log(imageChanger[imageAddressDict[subs[i]]] + "," + imageAddressDict[subs[i]] + "," + subs[i]);
                subjectImageObjectList[i].sprite = imageChanger[imageAddressDict[subs[i]]];
            }
            else
            {
                Debug.LogError("おい、指定されたキーが存在しないぞ\n存在しないキーは" + subs[i]);
            }
            subjectRankingList.Add(subs[i]);
        }
    }

    /// <summary>
    /// Updateで時間が来たらランキングのテキストとスプライトを変更する関数
    /// </summary>
    private void RankingChange()
    {
        subjectRankingList.Clear();
        activeSubjectList.Clear();

        //存在するオブジェクトを取得し、登録されていなければ登録
        activeSubjectList.AddRange(manager.GetAllSubject());

        //ランキングに入れて、テキストの中身の変更
        for (int i = 0; i < subjectTextObjectList.Length; i++)
        {
            if (activeSubjectList.Count != 0)
            {
                while (true)
                {
                    //存在するオブジェクトのタイプを取得
                    subjectEnum = activeSubjectList[Random.Range(0, activeSubjectList.Count)].GetSubjectType();
                    activeSubjectList.RemoveAt(i);

                    //同一のタイプが既にランキングにある場合にはまた回す
                    if (!subjectRankingList.Contains(subjectEnum))
                    {
                        break;
                    }

                    //もし最後の要素が消えてもその要素が既にランキングに存在した場合は
                    //別の要素を入れ、強制的にwhileを抜ける
                    if (activeSubjectList.Count != 0)
                    {
                        while (true)
                        {
                            //存在しないオブジェクトのタイプを取得
                            subjectEnum = subjectList[Random.Range(0, subjectList.Count)];

                            //同一のタイプが既にランキングにある場合にはまた回す
                            if (!subjectRankingList.Contains(subjectEnum))
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    //存在しないオブジェクトのタイプを取得
                    subjectEnum = subjectList[Random.Range(0, subjectList.Count)];

                    //同一のタイプが既にランキングにある場合にはまた回す
                    if (!subjectRankingList.Contains(subjectEnum))
                    {
                        break;
                    }
                }
            }

            //現在のキーがtextChangerとimageAddressDictに存在するか？
            if (textChanger.ContainsKey(subjectEnum) && imageAddressDict.ContainsKey(subjectEnum))
            {
                //テキストの変更
                subjectTextObjectList[i].text = textChanger[subjectEnum];
                //イメージの中身の変更
                Debug.Log(imageChanger[imageAddressDict[subjectEnum]] + "," + imageAddressDict[subjectEnum] + "," + subjectEnum);
                subjectImageObjectList[i].sprite = imageChanger[imageAddressDict[subjectEnum]];
            }
            else
            {
                Debug.LogError("おい、指定されたキーが存在しないぞ\n存在しないキーは" + subjectEnum);
            }

            subjectRankingList.Add(subjectEnum);
        }
    }

    /// <summary>
    /// テキストのセット関数
    /// </summary>
    private void SetText()
    {
        for (int i = 0; i < (int)SubjectEnum.END; i++)
        {
            subjectEnum = (SubjectEnum)i;

            switch (subjectEnum)
            {
                case SubjectEnum.UFO:
                    textChanger.Add(subjectEnum, "UFO");
                    break;
                case SubjectEnum.Sineitai:
                    textChanger.Add(subjectEnum, "親衛隊");
                    break;
                case SubjectEnum.MiddleAgeLady:
                    textChanger.Add(subjectEnum, "大阪のおばちゃん");
                    break;
                case SubjectEnum.Whale:
                    textChanger.Add(subjectEnum, "クジラ");
                    break;
                case SubjectEnum.Tornado:
                    textChanger.Add(subjectEnum, "竜巻");
                    break;
                case SubjectEnum.Idol:
                    textChanger.Add(subjectEnum, "アイドル");
                    break;
                case SubjectEnum.Arien:
                    textChanger.Add(subjectEnum, "宇宙人");
                    break;
                case SubjectEnum.SP:
                    textChanger.Add(subjectEnum, "SP");
                    break;
                case SubjectEnum.Cow:
                    textChanger.Add(subjectEnum, "牛");
                    break;
                case SubjectEnum.Zombie:
                    textChanger.Add(subjectEnum, "ゾンビ");
                    break;
                case SubjectEnum.END:
                    break;
            }
        }
    }

    /// <summary>
    /// イメージのスプライトのセット関数
    /// </summary>
    private void SetImage()
    {
        for (int i = 0; i < (int)SubjectEnum.END; i++)
        {
            subjectEnum = (SubjectEnum)i;

            switch (subjectEnum)
            {
                case SubjectEnum.UFO:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/UFO");
                    break;
                case SubjectEnum.Sineitai:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/Sineitai");
                    break;
                case SubjectEnum.MiddleAgeLady:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/Obahan");
                    break;
                case SubjectEnum.Whale:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/Whale");
                    break;
                case SubjectEnum.Tornado:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/Tornado");
                    break;
                case SubjectEnum.Idol:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/UnityChan");
                    break;
                case SubjectEnum.Arien:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/Gray");
                    break;
                case SubjectEnum.SP:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/SP");
                    break;
                case SubjectEnum.Cow:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/Cow");
                    break;
                case SubjectEnum.Zombie:
                    //spriteなので拡張子は要らない
                    imageAddressDict.Add(subjectEnum, "TrendSourceImage/Zombie");
                    break;
                case SubjectEnum.END:
                    break;
            }
            //Debug.Log(imageAddressDict[subjectEnum]);
            imageChanger.Add(imageAddressDict[subjectEnum], Resources.Load<Sprite>(imageAddressDict[subjectEnum]));
        }
    }

    private IEnumerator Timer()
    {
        updateText.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        updateText.SetActive(false);
    }

}
