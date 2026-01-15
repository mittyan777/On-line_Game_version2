using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor_control : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void cursorON()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void cursorOF()
    {
        Cursor.lockState = CursorLockMode.None;
    }

}
