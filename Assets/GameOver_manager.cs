using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameOver_manager : MonoBehaviour
{
    [SerializeField] GameObject Jail;
    [SerializeField] GameObject camera;
    AudioScript audioScript;

    [SerializeField] GameObject[] p1_obj;
    [SerializeField] GameObject[] p2_obj;

    public AudioClip BGM_File;
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

        if (MainGameManager.Player_GameObject_name[0] == "Player1(Clone)")
        {
            p1_obj[0].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[0] == "Player2(Clone)")
        {
            p1_obj[1].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[0] == "Player3(Clone)")
        {
            p1_obj[2].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[1] == "Player1(Clone)")
        {
            p2_obj[0].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[1] == "Player2(Clone)")
        {
            p2_obj[1].SetActive(true);
        }
        if (MainGameManager.Player_GameObject_name[1] == "Player3(Clone)")
        {
            p2_obj[2].SetActive(true);
        }
        if (GameObject.Find("Player1(Clone)") != null || GameObject.Find("Player2(Clone)") != null || GameObject.Find("Player3(Clone)") != null)
        {
            PhotonNetwork.LeaveRoom();
            Title_control.errorWindow = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
    }
}
