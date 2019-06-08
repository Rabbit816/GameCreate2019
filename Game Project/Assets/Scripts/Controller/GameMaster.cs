using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance;
    [SerializeField]
    private TurnCounter turnCounter;

    private int gameTurn;    // ターン数
    public int GameTurn { set { gameTurn = value; } get { return gameTurn; } }
    [SerializeField, Tooltip("ターン数の上限")]
    private int limitTurn;

    private int getCardCounter;    // 獲得したカードの枚数
    public int GetCardCounter { set { getCardCounter = value; } get { return getCardCounter; } }

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
        CardControl.Instance.SetCard();
    }

    private void Update()
    {
        turnCounter.GameTurn = gameTurn;
        if(getCardCounter == 52)
        {
            Debug.Log("ゲームクリアしました");
            Quit();
        }
        else if(gameTurn == limitTurn && fadeStartFlag)
        {
            StartCoroutine(Fade());
            fadeStartFlag = false;
        }
    }

    private void Quit()
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
        CardControl.Instance.ResetCard();
        turnCounter.CounterOn();
        fadeStartFlag = true;
    }

    IEnumerator Fade()
    {
        StartCoroutine(FadeControl.Instance.StartFadeOut());
        while (FadeControl.Instance.IsFading)
        {
            yield return new WaitForEndOfFrame();
        }
        CardControl.Instance.HideCards();
        turnCounter.CounterOff();

        StartCoroutine(FadeControl.Instance.StartFadeIn());
    }
}
