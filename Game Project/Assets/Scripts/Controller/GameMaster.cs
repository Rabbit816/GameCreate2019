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

    [SerializeField]
    private TurnCounter turnCounter;

    [SerializeField]
    private GameObject getCardBox;
    public GameObject GetCardBox { get { return getCardBox.transform.GetChild(0).gameObject; } }

    private int gameTurn;    // ターン数
    public int GameTurn { set { gameTurn = value; } get { return gameTurn; } }
    [SerializeField, Tooltip("ターン数の上限")]
    private int limitTurn;

    private int getCardCounter;    // 獲得したカードの枚数
    public int GetCardCounter { set { getCardCounter = value; } get { return getCardCounter; } }

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
        gameTurn = 0;
        turnCounter.LimitTurn = limitTurn;
    }

    private void Start()
    {
        card = CardControl.Instance;
        fade = FadeControl.Instance;
        result = ResultControl.Instance;
        card.SetCard(true);
        result.GameStart();
        card.GetCard.ResetGetCard();
        card.GetCard.HideGetCard();
    }

    private void Update()
    {
        turnCounter.GameTurn = gameTurn;
        if((getCardCounter == 52 || gameTurn == limitTurn) && fadeStartFlag)
        {
            fade.StartFade(GameOverAction);
            fadeStartFlag = false;
            if(getCardCounter == 52)
            {
                result.OutputResult(true);
            }
            else
            {
                result.OutputResult(false);
            }
        }
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
        getCardCounter = 0;
        gameTime = 0;
        timeFlag = false;
        card.SetCard(false);
        turnCounter.CounterOn();
        result.GameStart();
        fadeStartFlag = true;
        card.GetCard.ResetGetCard();
        card.GetCard.HideGetCard();
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
