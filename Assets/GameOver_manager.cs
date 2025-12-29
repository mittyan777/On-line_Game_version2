using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameOver_manager : MonoBehaviour
{
    [SerializeField] GameObject Jail;
    [SerializeField] GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.RotateAround(Jail.transform.position, Vector3.up, 8 * Time.deltaTime);
    }
}
