using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapCamera2 : MonoBehaviourPunCallbacks
{
    GameObject[] Target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (photonView.IsMine)
            {
                Target = GameObject.FindGameObjectsWithTag("Player2");
                transform.position = new Vector3(Target[0].transform.position.x, transform.position.y, Target[0].transform.position.z);
            }

        }
        catch (Exception ex)
        {
            Debug.LogWarning("CameraTarget Missing");
        }
    }
}
