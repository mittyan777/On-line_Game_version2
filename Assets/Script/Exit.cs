using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Exit : MonoBehaviour
{
    [SerializeField] GameObject InputField;
    [SerializeField]string Input_number;
    [SerializeField]int Exit_number;
    [SerializeField]Text[] number_text;
    public bool rock = true;
   
    string numberStr;
    char []firstChar;
    // Start is called before the first frame update
    void Start()
    {
        Exit_number = Random.Range(1000, 9999);
        numberStr = Exit_number.ToString();
       
    
    }

    // Update is called once per frame
    void Update()
    {
        Input_number = InputField.GetComponent<InputField>().text;
        number_text[0].text = numberStr.Substring(0, 1);
        number_text[1].text = numberStr.Substring(1, 1);
        number_text[2].text = numberStr.Substring(2, 1);
        number_text[3].text = numberStr.Substring(3, 1);
       
       
    }
    public void Confirmation()
    {
        if(Input_number == numberStr)
        {
            rock = false;
        }
    }
    public void Cancel()
    {
        InputField.SetActive(false);
    }
}
