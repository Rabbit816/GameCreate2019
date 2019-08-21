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
    private TitleControl title;

    [SerializeField]
    private GameObject getCardBoxMainObj;
    private GameObject getCardBox;
    private Button getCardBoxButton;
    public GameObject GetCardBox { get { return getCardBox; } }
    public bool GetCardButtonEnabled { set { getCardBoxButton.enabled = value; } get { return getCardBoxButton.enabled; } }

    private int gameTurn = 0;    // 経過ターン数
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
    private bool timeFlag = false;    //　時間計測を開始するフラグ
    public bool TimeFlag { set { timeFlag = value; } }

    private bool fadeStartFlag = false;    // フェードを管理するフラグ
    private bool firstGameFlag = true;    //　ゲームが初回かどうかをチェックするフラグ

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
        title = GetComponent<TitleControl>();

        // ゲーム初回起動時に実行する処理
        result.GameStart();
        card.GetCard.ResetGetCard();
        card.GetCard.HideGetCard();
        turnCounter.CounterOff();
        getCardBoxButton.enabled = false;
        title.ButtonActionSet();
    }

    private void Update()
    {
        // 残りターンをカウンターに反映
        turnCounter.LimitTurn = gamelimit;

        // 残りターン数が0になる又は、すべてのカードを獲得したらリザルトを出力
        if((getCardCounter == 52 || gamelimit == 0) && fadeStartFlag)
        {
            // 時間計測停止
            timeFlag = false;

            // 獲得カードリストは非表示にする
            getCardBoxButton.enabled = false;
            card.GetCard.ResetGetCard();
            card.GetCard.HideGetCard();

            // フェード処理と、リザルトの表示を実行
            fade.StartFade(GameOverAction);
            fadeStartFlag = false;
            result.OutputResult(isGameClear);
        }

        // 時間を計測
        if (timeFlag)
        {
            gameTime += Time.deltaTime;
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
    /// ゲームモード
    /// </summary>
    public enum GameMode
    {
        limit50,    // 50ターン以内にクリア
        limit75,    // 75ターン以内にクリア
        limit100,   // 100ターン以内にクリア
        limitInf    // 制限ターンなし
    }

    /// <summary>
    /// ゲーム開始ボタンの処理
    /// </summary>
    /// <param name="gameMode">ゲームモード</param>
    public void GameStartButton(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.limit50:
                limitTurn = 50;
                break;
            case GameMode.limit75:
                limitTurn = 75;
                break;
            default:
                limitTurn = 100;
                break;
        }

        gamelimit = limitTurn;

        if (gameMode == GameMode.limitInf)
        {
            limitGameMode = false;
            turnCounter.CounterOff();
        }
        else
        {
            limitGameMode = true;
            turnCounter.CounterOn();
        }

        // カードを並べる処理の開始
        card.SetCard(firstGameFlag);
        fadeStartFlag = true;
        if (firstGameFlag)
        {
            firstGameFlag = false;
        }
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
        card.SetCard(false);
        if (limitGameMode)
        {
            turnCounter.CounterOn();
        }
        result.GameStart();
        fadeStartFlag = true;
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
