using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GameStart_count : MonoBehaviourPun
{
    [SerializeField] GameObject[] ReadyUI;
    [SerializeField] Text count_text;
    [SerializeField] public float count = 5;
    int result;
    [SerializeField] Transform[] Start_pos;
    [SerializeField] GameObject Gamemanager;
    [SerializeField] GameObject StartText;
    bool time_count_trigger = false;

    // Start is called before the first frame update
    [PunRPC]
    void RPC_SetOpen(string target)
    {
        GetComponent<AudioSource>().Play();
        Gamemanager.GetComponent<MainGameManager>().sabaiba_count += 1;
        if (target == "Player")
        {
            ReadyUI[0].SetActive(true);
        }
        else if (target == "Player2")
        {
            ReadyUI[1].SetActive(true);
        }
        else if (target == "Killer")
        {
            ReadyUI[2].SetActive(true);
        }
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {

            count_text.gameObject.SetActive(true);
            time_count_trigger = true;
        }
    }
    [PunRPC]
    void RPC_SetOpen2(string target)
    {

        Gamemanager.GetComponent<MainGameManager>().sabaiba_count -= 1;
        if (target == "Player")
        {
            ReadyUI[0].SetActive(false);
        }
        else if (target == "Player2")
        {
            ReadyUI[1].SetActive(false);
        }
        else if (target == "Killer")
        {
            ReadyUI[2].SetActive(false);
        }

        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {

            count_text.gameObject.SetActive(true);
            time_count_trigger = true;
        }

    }
    [PunRPC]
    void RPC_SetOpen3(string target)
    {
        GetComponent<AudioSource>().Play();
        Gamemanager.GetComponent<MainGameManager>().killer_count += 1;
        if (target == "Player")
        {
            ReadyUI[0].SetActive(true);
        }
        else if (target == "Player2")
        {
            ReadyUI[1].SetActive(true);
        }
        else if (target == "Killer")
        {
            ReadyUI[2].SetActive(true);
        }

        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            count_text.gameObject.SetActive(true);
            time_count_trigger = true;
        }
    }
    [PunRPC]
    void RPC_SetOpen4(string target)
    {

        Gamemanager.GetComponent<MainGameManager>().killer_count -= 1;

        if (target == "Player")
        {
            ReadyUI[0].SetActive(false);
        }
        else if (target == "Player2")
        {
            ReadyUI[1].SetActive(false);
        }
        else if (target == "Killer")
        {
            ReadyUI[2].SetActive(false);
        }

        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            count_text.gameObject.SetActive(true);
            time_count_trigger = true;

        }

    }
    private void Update()
    {
        if (time_count_trigger == true)
        {
            int result = Mathf.FloorToInt(count);

            count_text.text = ($"{result}");
        }
        if (count <= 0)
        {
            if (Gamemanager.GetComponent<MainGameManager>().GameStart_trigger == false)
            {
                GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
                GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
                GameObject.FindWithTag("Killer").transform.position = Start_pos[2].position;
            }
            if (StartText != null) StartText.SetActive(true);
            count_text.gameObject.SetActive(false);
            Gamemanager.GetComponent<MainGameManager>().GameStart_trigger = true;
        }
    }
    public void SetOpen(string target)
    {
        photonView.RPC(nameof(RPC_SetOpen), RpcTarget.AllBuffered, target);
    }
    public void SetOpen2(string target)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_SetOpen2), RpcTarget.All, target);
    }
    public void SetOpen3(string target)
    {

        photonView.RPC(nameof(RPC_SetOpen3), RpcTarget.All, target);
    }
    public void SetOpen4(string target)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_SetOpen4), RpcTarget.All, target);
    }


}
