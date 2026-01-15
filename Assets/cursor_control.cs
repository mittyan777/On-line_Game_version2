using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor_control : MonoBehaviour
{
    [SerializeField] GameObject []target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool anyTrue = false;

        for (int i = 0; i < target.Length; i++)
        {
            if (target[i].activeSelf)   // true を見つけたら
            {
                anyTrue = true;
                break;
            }
        }

        Cursor.lockState = anyTrue ? CursorLockMode.None : CursorLockMode.Locked;



    }


}
