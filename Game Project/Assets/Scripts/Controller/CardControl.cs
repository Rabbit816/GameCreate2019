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

    // 盤面のカードを格納しておくオブジェクト
    [SerializeField]
    private GameObject allCards;

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
        // インスタンスの生成
        var cardInstance = Instantiate(this.cardObject);

        // インスタンスに情報を割り当てる
        var cardData = cardInstance.GetComponent<CardView>();
        var cardButton = cardInstance.GetComponent<UnityEngine.UI.Button>();

        // オブジェクトの位置を設定
        cardInstance.transform.SetParent(this.allCards.transform);
        cardInstance.transform.localPosition = new Vector3(0, 0, 0);

        // カードを表示
        cardData.OutputCard();
    }
}
