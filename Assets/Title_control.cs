using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_control : MonoBehaviour
{
    [SerializeField] GameObject camera;
    bool star_trigger;
    float Speed = 10;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fogDensity = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        if (star_trigger == false)
        {
            if (camera.transform.position.z >= 0)
            {
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, -60);
            }

        }
        else
        {
            if (RenderSettings.fogDensity > 0f && RenderSettings.fogDensity < 1f)
            {
                RenderSettings.fogDensity += 0.001f;
            }
        }
        if (camera.transform.position.z <= 37)
        {
            camera.transform.position += transform.forward * Speed * Time.deltaTime;
        }
        else
        {
            PhotonNetwork.LoadLevel("lobby");

        }

    }
    public void Gamestar()
    {
        if (star_trigger == false)
        {
            star_trigger = true;
            camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, 0);
        }
    }
}
