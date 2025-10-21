using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviourPunCallbacks
{
    GameObject []Target;
    [SerializeField]GameObject manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.GetComponent<MainGameManager>().Gamestart == true)
        {
            if (photonView.IsMine)
            {
                Target = GameObject.FindGameObjectsWithTag("Killer");
                transform.position = new Vector3(Target[0].transform.position.x, transform.position.y, Target[0].transform.position.z);
            }
        }
    }
}
