using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControl : MonoBehaviour
{
    public static CardControl Instance;

    // カードのImageデータ
    public List<Sprite> SpadeCards;
    public List<Sprite> CloverCards;
    public List<Sprite> HeartCards;
    public List<Sprite> DiaCards;
    public Sprite CardMain;

    // カードオブジェクトの元データ
    [SerializeField]
    private GameObject cardObject;

    // カードオブジェクト
    private GameObject[] allCardObjects = new GameObject[52];

    // 盤面のカードを格納しておくオブジェクト
    [SerializeField]
    private GameObject allCards;

    // トランプのシャッフル用の配列
    private int[] cards = new int[52];

    // カードの正誤判定
    private bool cardCheckFlag = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SetCard();
    }

    /// <summary>
    /// カードを画面上に並べる処理
    /// </summary>
    private void SetCard()
    {
        // シャッフル用の配列の準備
        for(int i = 0; i < cards.Length; i++) cards[i] = i;

        for (int i = 0; i < cards.Length; i++)
        {
            int tmp = cards[i];
            int index = Random.Range(0, cards.Length);
            cards[i] = cards[index];
            cards[index] = tmp;
        }

        // 情報の割り当て
        for(int i = 0; i < allCardObjects.Length; i++)
        {
            // インスタンスの生成
            allCardObjects[i] = Instantiate(this.cardObject);

            // インスタンスに情報を割り当てる
            var cardData = allCardObjects[i].GetComponent<CardView>();
            var cardButton = allCardObjects[i].GetComponent<UnityEngine.UI.Button>();
            if (0 <= cards[i] && cards[i] < 13)
            {
                cardData.CardNumber = cards[i];
                cardData.CardMark = "スペード";
                allCardObjects[i].name = cardData.CardMark + "：" + (cardData.CardNumber + 1);
            }
            else if (13 <= cards[i] && cards[i] < 26)
            {
                cardData.CardNumber = cards[i] - 13;
                cardData.CardMark = "クローバー";
                allCardObjects[i].name = cardData.CardMark + "：" + (cardData.CardNumber + 1);
            }
            else if (26 <= cards[i] && cards[i] < 39)
            {
                cardData.CardNumber = cards[i] - 26;
                cardData.CardMark = "ハート";
                allCardObjects[i].name = cardData.CardMark + "：" + (cardData.CardNumber + 1);
            }
            else
            {
                cardData.CardNumber = cards[i] - 39;
                cardData.CardMark = "ダイヤ";
                allCardObjects[i].name = cardData.CardMark + "：" + (cardData.CardNumber + 1);
            }
            cardData.CardId = i;
            // カードをクリックしたら実行する処理の追加
            cardButton.onClick.AddListener(() => cardData.OutputCard());
            cardButton.onClick.AddListener(() => CheckCard(cardData.CardNumber));
        }

        // オブジェクトのTransformを設定
        int count = 0;
        var startPos = new Vector3(-845, -300, 0);
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 13; j++)
            {
                allCardObjects[count].transform.SetParent(this.allCards.transform);
                allCardObjects[count].transform.localScale = new Vector3(0.15f, 0.15f);
                // X間隔140 Y間隔200
                allCardObjects[count].transform.localPosition = new Vector3(startPos.x + 140 * j, startPos.y + 200 * i, 0);
                count++;
            }
        }        
    }

    private void CheckCard(int cardNum)
    {
        if (!cardCheckFlag)
        {
            cardCheckFlag = true;
            return;
        }
    }
}
