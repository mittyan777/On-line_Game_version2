using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point_Move : MonoBehaviour
{
    //‰•œˆÚ“®
    Vector3 startPos;//’è‹`
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;//‰ŠúÀ•W‚Ì‘ã“ü
    }

    // Update is called once per frame
    void Update()
    {
  
        float posY = startPos.y + Mathf.Sin(Time.time * 2) / 4;//‰ŠúÀ•W{‰•œˆÚ“®‚ğposY‚É‘ã“ü

        transform.position = new Vector3(transform.position.x, posY, transform.position.z);
    }
}
