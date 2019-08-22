using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

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

    // カードオブジェクトの元データ
    [SerializeField]
    private Button cardObject;

    // カードオブジェクト
    private Button[] allCardObjects = new Button[52];
    private CardView[] allCardView = new CardView[52];
    
    // 盤面のカードを格納しておくオブジェクト
    [SerializeField]
    private GameObject allCards;

    // トランプのシャッフル用の配列
    private int[] cards = new int[52];

    // カードの正誤判定
    private bool cardCheckFlag = false;
    private List<int> cardNumList = new List<int>();
    private List<int> cardIdList = new List<int>();

    // 獲得したカード情報
    [SerializeField]
    private GetCardView getCard;
    public GetCardView GetCard { get { return getCard; } }

    private int cardFirstMoveCount = 0;
    private int cardSecondMoveCount = 0;
    public int CardSecondMoveCount { set { cardSecondMoveCount = value; } get { return cardSecondMoveCount; } }
    private int cardMoveCounter = 0;


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
    public void SetCard(bool isFirstGame)
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
        for(int i = 0; i < allCardView.Length; i++)
        {
            // インスタンスの生成
            if(allCardObjects[i] == null)
            {
                allCardObjects[i] = Instantiate(cardObject);
            }
            if(allCardView[i] == null)
            {
                allCardView[i] = allCardObjects[i].GetComponent<CardView>();
            }

            // インスタンスに情報を割り当てる
            var cardButton = allCardObjects[i];
            var cardView = allCardView[i];
            
            if (0 <= cards[i] && cards[i] < 13)
            {
                cardView.CardNumber = cards[i];
                cardView.CardMark = "スペード";
                cardView.CardSpriteData = spadeCards[cardView.CardNumber];
            }
            else if (13 <= cards[i] && cards[i] < 26)
            {
                cardView.CardNumber = cards[i] - 13;
                cardView.CardMark = "クローバー";
                cardView.CardSpriteData = cloverCards[cardView.CardNumber];
            }
            else if (26 <= cards[i] && cards[i] < 39)
            {
                cardView.CardNumber = cards[i] - 26;
                cardView.CardMark = "ハート";
                cardView.CardSpriteData = heartCards[cardView.CardNumber];
            }
            else
            {
                cardView.CardNumber = cards[i] - 39;
                cardView.CardMark = "ダイヤ";
                cardView.CardSpriteData = diaCards[cardView.CardNumber];
            }
        }

        // カードを1つのオブジェクトに格納
        foreach (Button button in allCardObjects)
        {
            button.transform.SetParent(allCards.transform);
            button.transform.localPosition = new Vector3(0, -1 * Screen.height, 0);
        }

        // 初回ゲーム時のみ実行
        if (isFirstGame)
        {
            int count = 0;
            var startPos = new Vector3(-845, 300, 0);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    var cardButton = allCardObjects[count];
                    var cardView = allCardView[count];
                    cardButton.transform.localScale = new Vector3(0.15f, 0.15f);
                    cardView.CardSetPos = new Vector3(startPos.x + 140 * j, startPos.y - 200 * i, 0);

                    cardView.CardId = count;    // カードの固有ID
                    cardButton.onClick.AddListener(() => cardView.CardOpen());    // カードをめくる処理の追加
                    cardButton.onClick.AddListener(() => CheckCard(cardView.CardNumber, cardView.CardId));    // ペア判定処理の追加

                    count++;
                }
            }
        }
        else
        {
            foreach (CardView view in allCardView)
            {
                view.ResetCard();
                view.gameObject.SetActive(true);
            }
        }

        foreach(Button btn in allCardObjects)
        {
            btn.enabled = false;
        }

        StartCoroutine(CardMove());
    }

    private IEnumerator CardMove()
    {
        int count = 0;
        cardMoveCounter = 0;

        // カード並べる処理を4列分繰り返す
        while (count < 4)
        {
            var card = new CardView[13];
            cardFirstMoveCount = 0;
            cardSecondMoveCount = 0;

            for (int i = 0; i < card.Length; i++)
            {
                card[i] = allCardView[cardMoveCounter];
                card[i].transform.DOLocalMove(new Vector3(-845, 300 - (200 * count), 0), 0.75f).OnComplete(() => { cardFirstMoveCount++; });
                cardMoveCounter++;
            }

            while (cardFirstMoveCount < 13)
            {
                yield return new WaitForEndOfFrame();
            }

            foreach (CardView view in card)
            {
                view.SetCardPosition();
            }

            while(cardSecondMoveCount < 13)
            {
                yield return new WaitForEndOfFrame();
            }
            
            count++;
        }

        // カードのクリックを許可
        foreach (Button card in allCardObjects)
        {
            card.enabled = true;
        }

        GameMaster.Instance.TimeFlag = true;
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
            // ペアだった場合
            StartCoroutine(DirectionToCard(1.0f, true));
        }
        else
        {
            // ペアでなかった場合
            StartCoroutine(DirectionToCard(1.0f, false));
        }
    }

    /// <summary>
    /// めくったカードに指示を出す処理
    /// </summary>
    /// <param name="time">実行開始時間(秒)</param>
    /// <param name="flag">true=消える、false=裏返す</param>
    /// <returns></returns>
    IEnumerator DirectionToCard(float time, bool flag)
    {
        yield return new WaitForSeconds(time);
        for(int i = 0; i < cardIdList.Count; i++)
        {
            var card = allCardObjects[cardIdList[i]];
            var cardView = allCardView[cardIdList[i]];

            if (flag)
            {
                card.enabled = false;
                card.transform.SetParent(GameMaster.Instance.GetCardBox.transform);
                card.transform.DOLocalMove(new Vector3(0, 0, 0), 0.75f);
                getCard.OutputGetCard(cardView.CardSpriteData);
                if (!GameMaster.Instance.GetCardButtonEnabled)
                {
                    GameMaster.Instance.GetCardButtonEnabled = true;
                }
                GameMaster.Instance.GetCardCounter++;
            }
            else
            {
                cardView.CardClose();    // カードを裏返す
            }
        }

        while(allCardView[cardIdList[cardIdList.Count - cardIdList.Count]].IsCardTurning && allCardView[cardIdList.Count - 1].IsCardTurning)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach(Button button in allCardObjects)
        {
            button.enabled = true;    // カードのクリックを有効にする
        }

        cardIdList.Clear();
        cardNumList.Clear();
        cardCheckFlag = false;
        GameMaster.Instance.GameTurn++;
    }

    /// <summary>
    /// 盤面のカードを全て非表示にする
    /// </summary>
    public void HideCards()
    {
        foreach(Button button in allCardObjects)
        {
            button.gameObject.SetActive(false);
        }
    }
}
