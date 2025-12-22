using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameStart_count : MonoBehaviourPun
{
    [SerializeField] Text sabaiba_text;
    [SerializeField] Text killer_text;
    [SerializeField] Transform[] Start_pos;
    [SerializeField]GameObject Gamemanager;
    // Start is called before the first frame update
    [PunRPC]
    void RPC_SetOpen()
    {

        Gamemanager.GetComponent<MainGameManager>().sabaiba_count += 1;
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2)
        {
            sabaiba_text.text = "サバイバー準備OK";
        }
        else
        {
            sabaiba_text.text = Gamemanager.GetComponent<MainGameManager>().sabaiba_count.ToString();
        }
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
            GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
            GameObject.FindWithTag("Killer").transform.position = Start_pos[3].position;
        }
    }
    [PunRPC]
    void RPC_SetOpen2()
    {

        Gamemanager.GetComponent<MainGameManager>().sabaiba_count -= 1;
      sabaiba_text.text = Gamemanager.GetComponent<MainGameManager>().sabaiba_count.ToString();
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
            GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
            GameObject.FindWithTag("Killer").transform.position = Start_pos[3].position;
        }

    }
    [PunRPC]
    void RPC_SetOpen3()
    {

        Gamemanager.GetComponent<MainGameManager>().killer_count += 1;
        if (Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            killer_text.text = "キラー準備OK";
        }
        else
        {
            killer_text.text = Gamemanager.GetComponent<MainGameManager>().killer_count.ToString();
        }
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            sabaiba_text.text = "";
            killer_text.text = "";
            GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
            GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
            GameObject.FindWithTag("Killer").transform.position = Start_pos[2].position;
        }
    }
    [PunRPC]
    void RPC_SetOpen4()
    {

        Gamemanager.GetComponent<MainGameManager>().killer_count -= 1;
        killer_text.text = Gamemanager.GetComponent<MainGameManager>().killer_count.ToString();
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
            GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
            GameObject.FindWithTag("Killer").transform.position = Start_pos[3].position;
        }

    }
    private void Update()
    {
       
    }
    public void SetOpen()
    {
        photonView.RPC(nameof(RPC_SetOpen), RpcTarget.AllBuffered);
    }
    public void SetOpen2()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_SetOpen2), RpcTarget.All);
    }
    public void SetOpen3()
    {

        photonView.RPC(nameof(RPC_SetOpen3), RpcTarget.All);
    }
    public void SetOpen4()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC(nameof(RPC_SetOpen4), RpcTarget.All);
    }


}
