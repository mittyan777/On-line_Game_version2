using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamestart_count_controller : MonoBehaviour
{
    [SerializeField] GameObject GameStart_count;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void count()
    {
        if (gameObject.name == "Gamestart_count")
        {
            GameStart_count.GetComponent<GameStart_count>().count -= 1;
        }
        if (gameObject.name == "StartText")
        {
            Destroy(gameObject);
        }

    }

}
