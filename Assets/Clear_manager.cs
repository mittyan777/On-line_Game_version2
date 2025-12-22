using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear_manager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Player2;
    [SerializeField] GameObject killer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            photonView.RPC(nameof(PlayerDWS), RpcTarget.All);
          
        }
  
        if (GameObject.FindGameObjectWithTag("Player2") != null)
        {
            Player2 = GameObject.FindGameObjectWithTag("Player2");
            photonView.RPC(nameof(PlayerDWS2), RpcTarget.All);
        }
   
        if (GameObject.FindGameObjectWithTag("Killer") != null)
        {
            killer = GameObject.FindGameObjectWithTag("Killer");
            photonView.RPC(nameof(PlayerDWS3), RpcTarget.All);
        }
 
    }
    [PunRPC]
    void PlayerDWS()
    {
        Player.SetActive(false);
    }
    [PunRPC]
    void PlayerDWS2()
    {
        Player2.SetActive(false);
    }
    [PunRPC]
    void PlayerDWS3()
    {
        killer.SetActive(false);
    }
}
