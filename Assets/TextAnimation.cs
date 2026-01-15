using System.Collections;
using System.Xml;
using TMPro;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] string[] Story;
    [SerializeField] int Story_count;
    bool reset = false;

    void Start()
    {
        StartCoroutine(Simple());
        MeshRenderer renderer = tmpText.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "UI"; renderer.sortingOrder = 10;
    }

    private IEnumerator Simple()
    {
        // 文字の表示数を0に(テキストが表示されなくなる)
        tmpText.maxVisibleCharacters = 0;

        // テキストの文字数分ループ
        for (var i = 0; i < tmpText.text.Length; i++)
        {
            // 一文字ごとに0.2秒待機
            yield return new WaitForSeconds(0.2f);

            // 文字の表示数を増やしていく
            tmpText.maxVisibleCharacters = i + 1;
        }
        if (Story_count >= 0)
        {
            reset = false;
            Story_count -= 1;
            tmpText.text = Story[Story_count];
            StartCoroutine(Simple());
        }
       
       
    }
    private void Update()
    {
        if (Story_count < 0 && reset == false)
        {
            reset = true;
            Story_count = 3;
            tmpText.text = Story[3];
            StartCoroutine(Simple());
        }
    }
}