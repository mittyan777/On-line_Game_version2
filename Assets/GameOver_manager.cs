using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameOver_manager : MonoBehaviour
{
    string[] a;
    [SerializeField] TextMeshProUGUI[] Game_completer_name;
    [SerializeField] TextMeshProUGUI[] Game_completer_mesh;
    [SerializeField] TextMeshProUGUI[] Player_name;

    [SerializeField] GameObject Jail;
    [SerializeField] GameObject camera;
    AudioScript audioScript;

    [SerializeField] GameObject[] p1_obj;
    [SerializeField] GameObject[] p2_obj;

    public AudioClip BGM_File;

    bool p1;
    bool p2;
    // Start is called before the first frame update
    void Start()
    {
        audioScript = GameObject.Find("BGM").GetComponent<AudioScript>();
        audioScript.Change_PlayAudio(BGM_File);
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.RotateAround(Jail.transform.position, Vector3.up, 8 * Time.deltaTime);

        if (GameObject.Find("Player1(Clone)") != null || GameObject.Find("Player2(Clone)") != null || GameObject.Find("Player3(Clone)") != null)
        {
            PhotonNetwork.LeaveRoom();
            Title_control.errorWindow = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }



        if (MainGameManager.Player_GameObject_name[0] == "Player1(Clone)" && p1 == false)
        {
            p1_obj[0].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[0] == "Player2(Clone)" && p1 == false)
        {
            p1_obj[1].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[0] == "Player3(Clone)" && p1 == false)
        {
            p1_obj[2].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[1] == "Player1(Clone)" && p2 == false)
        {
            p2_obj[0].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[1] == "Player2(Clone)" && p2 == false)
        {
            p2_obj[1].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[1] == "Player3(Clone)" && p2 == false)
        {
            p2_obj[2].SetActive(true);
        }

        // プレイヤー1が脱出したか
        p1 = MainGameManager.Game_completer_name[0] == "Player";

        // プレイヤー2が脱出したか
        p2 = MainGameManager.Game_completer_name[1] == "Player2";

        // 結果表示
        Game_completer_mesh[0].text = p1 ? "脱出" : "脱出失敗";
        Game_completer_mesh[1].text = p2 ? "脱出" : "脱出失敗";

        Game_completer_name[0].text = ($"{MainGameManager.Player_name[0]}");
        Game_completer_name[1].text = ($"{MainGameManager.Player_name[1]}");
        Debug.Log("P1 = " + MainGameManager.Game_completer_name[0]);
        Debug.Log("P2 = " + MainGameManager.Game_completer_name[1]);
    }
}
