using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance;

    private CardControl card;
    private FadeControl fade;
    private ResultControl result;
    private TurnCounter turnCounter;

    [SerializeField]
    private GameObject getCardBoxMainObj;
    private GameObject getCardBox;
    private Button getCardBoxButton;
    public GameObject GetCardBox { get { return getCardBox; } }

    private int gameTurn = 0;    // 経過ターン数
    [SerializeField]
    private int limitTurn = 0;    // 設定した制限ターン数
    private int gamelimit = 0;    // ゲーム終了までのターン数
    private bool limitGameMode = true;    // ゲームモードがターン制限ありどうか
    public int GameTurn
    {
        set
        {
            gameTurn = value;
            if (limitGameMode && limitTurn > 0)
            {
                gamelimit--;
            }
        }
        get
        {
            return gameTurn;
        }
    }

    private int getCardCounter;    // 獲得したカードの枚数
    public int GetCardCounter { set { getCardCounter = value; } get { return getCardCounter; } }
    private bool isGameClear    // ゲームをクリアしたかの判定
    {
        get
        {
            if(getCardCounter == 52)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private float gameTime;    // 経過時間
    public float GameTime { get { return gameTime; } }
    private bool timeFlag = false;
    public bool TimeFlag { set { timeFlag = value; } }

    private bool fadeStartFlag = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // ゲーム初回起動時に取得する要素
        card = CardControl.Instance;
        fade = GetComponent<FadeControl>();
        result = GetComponent<ResultControl>();
        turnCounter = GetComponent<TurnCounter>();
        getCardBox = getCardBoxMainObj.transform.GetChild(0).gameObject;
        getCardBoxButton = getCardBoxMainObj.transform.GetChild(1).GetComponent<Button>();

        // ゲーム初回起動時に実行する処理
        card.SetCard(true);
        result.GameStart();
        card.GetCard.ResetGetCard();
        card.GetCard.HideGetCard();
        gamelimit = limitTurn;
    }

    private void Update()
    {
        // 残りターンをカウンターに反映
        turnCounter.LimitTurn = gamelimit;

        // 残りターン数が0になる又は、すべてのカードを獲得したらリザルトを出力
        if((getCardCounter == 52 || gamelimit == 0) && fadeStartFlag)
        {
            // 獲得カードリストは非表示にする
            getCardBoxButton.enabled = false;
            card.GetCard.ResetGetCard();
            card.GetCard.HideGetCard();

            // フェード処理と、リザルトの表示を実行
            fade.StartFade(GameOverAction);
            fadeStartFlag = false;
            result.OutputResult(isGameClear);
        }
        // 残りターンが1ターン以上で、すべてのカードを獲得していないなら時間を計測
        else
        {
            if (timeFlag)
            {
                gameTime += Time.deltaTime;
            }
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
    UnityEngine.Application.Quit();
#endif
    }

    /// <summary>
    /// ゲームを初期化してスタート
    /// </summary>
    public void ResetGame()
    {
        gameTurn = 0;
        gamelimit = limitTurn;
        getCardCounter = 0;
        gameTime = 0;
        timeFlag = false;
        card.SetCard(false);
        turnCounter.CounterOn();
        result.GameStart();
        fadeStartFlag = true;
        getCardBoxButton.enabled = true;
        getCardBox.SetActive(true);
    }

    /// <summary>
    /// GameOverになったら実行する処理
    /// </summary>
    private void GameOverAction()
    {
        card.HideCards();
        turnCounter.CounterOff();
        result.GameEnd();
        getCardBox.SetActive(false);
    }
}
