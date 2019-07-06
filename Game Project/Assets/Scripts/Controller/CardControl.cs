﻿using System.Collections;
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
    private UnityEngine.Events.UnityAction[] buttonAction1 = new UnityEngine.Events.UnityAction[52], buttonAction2 = new UnityEngine.Events.UnityAction[52];
    
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
            cardView.CardId = i;
        }
        // カードをクリックしたら実行する処理の追加
        for(int i = 0; i < buttonAction1.Length; i++)
        {
            if(buttonAction1[i] == null)
            {
                buttonAction1[i] = () => allCardView[i].CardOpen();
                allCardObjects[i].onClick.AddListener(buttonAction1[i]);
            }
        }
        for(int i = 0; i < buttonAction2.Length; i++)
        {
            if(buttonAction2[i] == null)
            {
                buttonAction2[i] = () => CheckCard(allCardView[i].CardNumber, allCardView[i].CardId);
                allCardObjects[i].onClick.AddListener(buttonAction2[i]);
            }
        }

        // オブジェクトのTransformを設定
        int count = 0;
        var startPos = new Vector3(-845, 300, 0);
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 13; j++)
            {
                allCardObjects[count].transform.SetParent(this.allCards.transform);
                allCardObjects[count].transform.localScale = new Vector3(0.15f, 0.15f);
                // X間隔140 Y間隔200
                allCardView[count].CardSetPos = new Vector3(startPos.x + 140 * j, startPos.y - 200 * i, 0);
                allCardObjects[count].transform.localPosition = new Vector3(0, -800, 0);
                count++;
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
        var card = new CardView[13];
        int count = 0;
        for(int i = 0; i < card.Length; i++)
        {
            card[i] = allCardView[i];
            card[i].transform.DOLocalMove(new Vector3(-845, 300 * (1 - count), 0), 0.75f).OnComplete(() => { cardFirstMoveCount++; });
        }

        while(cardFirstMoveCount == 13)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach(CardView view in card)
        {
            view.SetCardPosition();
        }

        Debug.Log("MoveEnd");
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
            var card = allCardView[cardIdList[i]];

            if (flag)
            {
                card.RemoveCard();    // カードを非表示にする
                getCard.OutputGetCard(card.CardSpriteData);
                GameMaster.Instance.GetCardCounter++;
            }
            else
            {
                card.CardClose();    // カードを裏返す
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
        allCards.SetActive(false);
    }

    /// <summary>
    /// カードを再配置する処理
    /// </summary>
    public void ResetCard()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int tmp = cards[i];
            int index = Random.Range(0, cards.Length);
            cards[i] = cards[index];
            cards[index] = tmp;
        }
        for (int i = 0; i < allCardView.Length; i++)
        {
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

        allCards.SetActive(true);

        foreach(Button button in allCardObjects)
        {
            button.enabled = true;
        }
        foreach(CardView view in allCardView)
        {
            view.ResetCard();
        }

        cardCheckFlag = false;
    }
}
