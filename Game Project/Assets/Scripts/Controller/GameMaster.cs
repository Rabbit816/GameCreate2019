using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance;

    private CardControl card;
    public CardControl Card { get { return card; } }
    private FadeControl fade;
    private ResultControl result;
    private TurnCounter turnCounter;
    private TitleControl title;

    [SerializeField, Tooltip("獲得したカードを格納するオブジェクト")]
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

    [SerializeField, Tooltip("メニューボタン")]
    private Button menuButton;
    [SerializeField, Tooltip("メニューボタンで表示されるオブジェクト")]
    private GameObject menuObject;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        // ゲーム初回起動時に取得する要素
        card = GetComponent<CardControl>();
        fade = GetComponent<FadeControl>();
        result = GetComponent<ResultControl>();
        turnCounter = GetComponent<TurnCounter>();
        getCardBox = getCardBoxMainObj.transform.GetChild(0).gameObject;
        getCardBoxButton = getCardBoxMainObj.transform.GetChild(1).GetComponent<Button>();
        title = GetComponent<TitleControl>();
    }

    private void Start()
    {
        // ゲーム初回起動時に実行する処理
        result.GameStart();
        card.GetCard.ResetGetCard();
        card.GetCard.GetCardListActive(false);
        turnCounter.CounterActive(false);
        getCardBoxButton.enabled = false;
        title.ButtonActionSet();
        MenuButtonActive(false);
        menuObject.SetActive(false);
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
            card.GetCard.GetCardListActive(false);

            // メニューを非表示及び無効
            MenuButtonActive(false);
            menuObject.SetActive(false);

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
            turnCounter.CounterActive(false);
        }
        else
        {
            limitGameMode = true;
            turnCounter.CounterActive(true);
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
            turnCounter.CounterActive(true);
        }
        result.GameStart();
        fadeStartFlag = true;
    }

    /// <summary>
    /// タイトルに戻るボタンの処理
    /// </summary>
    public void ReturnTitle()
    {
        gameTurn = 0;
        getCardCounter = 0;
        gameTime = 0;
        result.GameStart();
        title.ReturnTitle();
    }

    /// <summary>
    /// GameOverになったら実行する処理
    /// </summary>
    private void GameOverAction()
    {
        card.AllCardsActive(false);
        turnCounter.CounterActive(false);
        result.GameEnd();
    }

    /// <summary>
    /// メニューボタンを押したら呼び出される処理
    /// </summary>
    public void OpenMenuButton()
    {
        // タイマーを一時的に停止
        timeFlag = false;

        // カードのクリックを一時的に無効
        card.CardClick(false);

        //　メニューの表示
        menuObject.SetActive(true);

        //　メニューボタンのクリックを無効
        menuButton.enabled = false;

        // 獲得したカードリストを非表示
        getCardBoxButton.enabled = false;
        card.GetCard.GetCardListActive(false);
    }

    /// <summary>
    /// メニューのリスタートボタンの処理
    /// </summary>
    public void RestartMenuButton()
    {
        card.GetCard.ResetGetCard();
        ResetGame();
        MenuButtonActive(false);
        menuObject.SetActive(false);
    }

    /// <summary>
    /// メニューのタイトルに戻るボタンの処理
    /// </summary>
    public void TitleMenuButton()
    {
        gameTurn = 0;
        getCardCounter = 0;
        gameTime = 0;
        title.ReturnTitle();
        card.AllCardsActive(false);
        turnCounter.CounterActive(false);
        card.GetCard.ResetGetCard();
        MenuButtonActive(false);
        menuObject.SetActive(false);
    }

    /// <summary>
    /// メニューを閉じるボタンの処理
    /// </summary>
    public void CloseMenuButton()
    {
        // メニューを非表示
        menuObject.SetActive(false);

        // カードのクリックを有効
        card.CardClick(true);

        // タイマーの計測を再開
        timeFlag = true;

        // メニューボタンのクリックを有効
        menuButton.enabled = true;

        // 獲得したカードリストのボタンを有効
        if(getCardCounter > 0)
        {
            getCardBoxButton.enabled = true;
        }
    }

    /// <summary>
    /// メニューボタンのオンオフ
    /// </summary>
    /// <param name="b">true=表示, false=非表示</param>
    public void MenuButtonActive(bool b)
    {
        menuButton.gameObject.SetActive(b);
        if (!menuButton.enabled)
        {
            menuButton.enabled = true;
        }
    }
}
