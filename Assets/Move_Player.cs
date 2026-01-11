using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Player : MonoBehaviour
{
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speed += 90 * Time.deltaTime;
      GetComponent<RectTransform>().position = new Vector3(speed, GetComponent<RectTransform>().position.y, 0);
    }
}
