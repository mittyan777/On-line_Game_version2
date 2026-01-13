using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject[] _sliderImage;
    [SerializeField] private UnityEngine.UI.Image testImage;
    [SerializeField] private UnityEngine.UI.Image[] sliderImage;
    [SerializeField] private GameObject loadTEXTQparent;
    Animator animator;

    GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _loadingUI.SetActive(true);

        // ����GameObject�ɃA�^�b�`����Ă���Button���擾
        testImage = _loadingUI.GetComponent<UnityEngine.UI.Image>();

        sliderImage[1] = _sliderImage[1].GetComponent<UnityEngine.UI.Image>();
        sliderImage[2] = _sliderImage[2].GetComponent<UnityEngine.UI.Image>();

        animator = loadTEXTQparent.GetComponent<Animator>();


        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {


        Color slider2 = sliderImage[1].color;
        Color slider3 = sliderImage[2].color;
        _slider.value += 10 * Time.deltaTime;
        if (_slider.value == 100)
        {
            animator.SetBool("end", true);
            _loadingUI.GetComponent<Animator>().enabled = true;


            slider2.a -= 1f * Time.deltaTime;
            slider3.a -= 1f * Time.deltaTime;

        }
        if (_slider.value >= 70)
        {
            gameManager.GetComponent<MainGameManager>().Gamestart = true;
        }



        sliderImage[1].color = slider2;
        sliderImage[2].color = slider3;
    }
}
