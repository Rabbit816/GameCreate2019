using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardControl : MonoBehaviour
{
    public static CardControl Instance;

    // カードのImageデータ
    [SerializeField]
    private List<Sprite> spadeCards;
    [SerializeField]
    private List<Sprite> cloverCards;
    [SerializeField]
    private List<Sprite> heartCards;
    [SerializeField]
    private List<Sprite> diaCards;
    [SerializeField]
    private Sprite cardMainSprite;

    // カードオブジェクトの元データ
    [SerializeField]
    private Button cardObject;

    // カードオブジェクト
    private Button[] allCardObjects = new Button[52];

    // 盤面のカードを格納しておくオブジェクト
    [SerializeField]
    private GameObject allCards;

    // トランプのシャッフル用の配列
    private int[] cards = new int[52];

    // カードの正誤判定
    private bool cardCheckFlag = false;
    private List<int> cardNumList = new List<int>();
    private List<int> cardIdList = new List<int>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// カードを画面上に並べる処理
    /// </summary>
    public void SetCard()
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
            allCardObjects[i] = Instantiate(cardObject);

            // インスタンスに情報を割り当てる
            var cardData = allCardObjects[i].GetComponent<CardView>();
            cardData.MainSpriteData = cardMainSprite;
            if (0 <= cards[i] && cards[i] < 13)
            {
                cardData.CardNumber = cards[i];
                cardData.CardMark = "スペード";
                cardData.CardSpriteData = spadeCards[cardData.CardNumber];
            }
            else if (13 <= cards[i] && cards[i] < 26)
            {
                cardData.CardNumber = cards[i] - 13;
                cardData.CardMark = "クローバー";
                cardData.CardSpriteData = cloverCards[cardData.CardNumber];
            }
            else if (26 <= cards[i] && cards[i] < 39)
            {
                cardData.CardNumber = cards[i] - 26;
                cardData.CardMark = "ハート";
                cardData.CardSpriteData = heartCards[cardData.CardNumber];
            }
            else
            {
                cardData.CardNumber = cards[i] - 39;
                cardData.CardMark = "ダイヤ";
                cardData.CardSpriteData = diaCards[cardData.CardNumber];
            }
            cardData.CardId = i;
            // カードをクリックしたら実行する処理の追加
            allCardObjects[i].onClick.AddListener(() => cardData.OutputData());
            allCardObjects[i].onClick.AddListener(() => CheckCard(cardData.CardNumber, cardData.CardId));
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

    /// <summary>
    /// カードの正誤判定の処理
    /// </summary>
    /// <param name="cardNum"></param>
    private void CheckCard(int cardNum, int cardId)
    {
        cardNumList.Add(cardNum);
        cardIdList.Add(cardId);

        if (!cardCheckFlag)
        {
            allCardObjects[cardId].enabled = false;
            cardCheckFlag = true;
            return;
        }

        foreach(Button button in allCardObjects)
        {
            button.enabled = false;    // カードのクリックを無効にする
        }

        if(cardNumList.Distinct().Count() == 1)
        {
            // 
            StartCoroutine(directionToCard(0.75f, true));
        }
        else
        {
            //
            StartCoroutine(directionToCard(0.75f, false));
        }
        GameMaster.Instance.GameTurn++;
        cardCheckFlag = false;
    }

    /// <summary>
    /// めくったカードに指示を出す処理
    /// </summary>
    /// <param name="time">実行開始時間(秒)</param>
    /// <param name="flag">true=消える、false=裏返す</param>
    /// <returns></returns>
    IEnumerator directionToCard(float time, bool flag)
    {
        yield return new WaitForSeconds(time);
        for(int i = 0; i < cardIdList.Count; i++)
        {
            var cardData = allCardObjects[cardIdList[i]].GetComponent<CardView>();
            if (flag)
            {
                cardData.RemoveCard();    // カードを非表示にする
            }
            else
            {
                cardData.ReturnCard();    // カードを裏返す
            }
        }

        foreach(Button button in allCardObjects)
        {
            button.enabled = true;    // カードのクリックを有効にする
        }

        cardIdList.Clear();
        cardNumList.Clear();
    }
}
