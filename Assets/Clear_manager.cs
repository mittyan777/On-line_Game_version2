using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Clear_manager : MonoBehaviourPunCallbacks
{
    string[] a;
    [SerializeField] TextMeshProUGUI[] Game_completer_name;
    [SerializeField] TextMeshProUGUI[] Game_completer_mesh;
    [SerializeField] TextMeshProUGUI[] Player_name;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Player2;
    [SerializeField] GameObject control_P;

    [SerializeField] string TitleScene_name = "Title";
    // Start is called before the first frame update
    void Start()
    {
        // a = MainGameManager.Game_completer_name;
        //PhotonNetwork.LeaveRoom();
        // ??????????????????????????????
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Player1(Clone)") != null || GameObject.Find("Player2(Clone)") != null || GameObject.Find("Player3(Clone)") != null)
        {
            PhotonNetwork.LeaveRoom();
            Title_control.errorWindow = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
        Debug.Log(MainGameManager.Game_completer_name[0]);

        // ƒvƒŒƒCƒ„[1‚ª’Eo‚µ‚½‚©
        bool p1 = MainGameManager.Game_completer_name[0] == "Player";

        // ƒvƒŒƒCƒ„[2‚ª’Eo‚µ‚½‚©
        bool p2 = MainGameManager.Game_completer_name[1] == "Player2";

        // Œ‹‰Ê•\Ž¦
        Game_completer_mesh[0].text = p1 ? "’Eo" : "’EoŽ¸”s";
        Game_completer_mesh[1].text = p2 ? "’Eo" : "’EoŽ¸”s";

        Debug.Log("P1 = " + MainGameManager.Game_completer_name[0]);
        Debug.Log("P2 = " + MainGameManager.Game_completer_name[1]);


        if (control_P.transform.position.z >= -37)
        {
            control_P.transform.position = new Vector3(control_P.transform.position.x, control_P.transform.position.y, -71);
        }
        control_P.transform.position += transform.forward * 5 * Time.deltaTime;

    }
}
