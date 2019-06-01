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

    // フェードテクスチャ
    private Texture2D blackTexture;
    private float fadeAlpha = 0;
    private bool isFading = false;
    private bool fadeStartFlag = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        gameTurn = 0;
        turnCounter.LimitTurn = limitTurn;

        blackTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
        blackTexture.ReadPixels(new Rect(0, 0, 32, 32), 0, 0, false);
        blackTexture.SetPixel(0, 0, Color.white);
        blackTexture.Apply();
    }

    private void OnGUI()
    {
        if (!isFading) return;

        // 黒いテクスチャの描画
        GUI.color = new Color(0, 0, 0, fadeAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
    }

    private void Start()
    {
        CardControl.Instance.SetCard();
    }

    private void Update()
    {
        turnCounter.GameTurn = gameTurn;
        if(gameTurn >= limitTurn && fadeStartFlag)
        {
            StartCoroutine(FadeScene(1.5f));
            fadeStartFlag = false;
        }
    }

    private IEnumerator FadeScene(float interval)
    {
        isFading = true;

        // 暗くする
        float time = 0;
        while (time <= interval)
        {
            fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        // 明るくする
        time = 0;
        while (time <= interval)
        {
            fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        isFading = false;
    }
}
