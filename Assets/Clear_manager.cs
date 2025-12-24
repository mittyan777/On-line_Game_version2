using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clear_manager : MonoBehaviourPunCallbacks
{
    string[] a;
    [SerializeField] Text[] Game_completer_name;
    [SerializeField] TextMesh [] Game_completer_mesh;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Player2;
    // Start is called before the first frame update
    void Start()
    {
       // a = MainGameManager.Game_completer_name;
        //PhotonNetwork.LeaveRoom();
    }

    // Update is called once per frame
    void Update()
    {
        Game_completer_name[0].text = MainGameManager.Game_completer_name[0];
        Game_completer_name[1].text = MainGameManager.Game_completer_name[1];
        if (MainGameManager.Game_completer_name[0] == "Player")
        {
            Game_completer_mesh[0].text = "íEèoê¨å˜";
        }
        else if (MainGameManager.Game_completer_name[1] == "Player")
        {
            Game_completer_mesh[0].text = "íEèoê¨å˜";
        }
        else
        {
            Game_completer_mesh[0].text = "íEèoé∏îs";
        }
        if (MainGameManager.Game_completer_name[0] == "Player2")
        {
            Game_completer_mesh[1].text = "íEèoê¨å˜";
        }
        else if (MainGameManager.Game_completer_name[1] == "Player2")
        {
            Game_completer_mesh[1].text = "íEèoê¨å˜";
        }
        else
        {
            Game_completer_mesh[1].text = "íEèoé∏îs";
        }


    }
}
