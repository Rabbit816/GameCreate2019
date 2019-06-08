using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeControl : MonoBehaviour
{
    public static FadeControl Instance;

    [SerializeField]
    private float interval;    // 透明度が変わるスピードを管理
    private float time;
    private float red, green, blue, alpha;    // パネルの色、不透明度を管理

    [SerializeField]
    private Image fadeImage;    // 透明度を変更するパネルのイメージ

    private bool isFading = false;
    public bool IsFading { get { return isFading; } set { isFading = false; } }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alpha = fadeImage.color.a;
    }

    void Update()
    {
        SetAlpha();
    }

    public IEnumerator StartFadeIn()
    {
        isFading = true;
        time = 0;
        while(time <= interval)
        {
            alpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }
        fadeImage.enabled = false;
        isFading = false;
    }

    public IEnumerator StartFadeOut()
    {
        isFading = true;
        fadeImage.enabled = true;
        time = 0;
        while(time <= interval)
        {
            alpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }
        isFading = false;
    }

    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alpha);
    }
}
