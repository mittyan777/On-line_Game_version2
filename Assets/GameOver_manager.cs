using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameOver_manager : MonoBehaviour
{
    [SerializeField] GameObject Jail;
    [SerializeField] GameObject camera;
    AudioScript audioScript;

    public AudioClip BGM_File;
    // Start is called before the first frame update
    void Start()
    {
        audioScript = GameObject.Find("BGM").GetComponent<AudioScript>();
        audioScript.Change_PlayAudio(BGM_File);
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.RotateAround(Jail.transform.position, Vector3.up, 8 * Time.deltaTime);
    }
}
