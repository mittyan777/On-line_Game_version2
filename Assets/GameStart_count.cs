using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameStart_count : MonoBehaviourPun
{
    [SerializeField] Text sabaiba_text;
    [SerializeField] Text killer_text;
    [SerializeField] Transform[] Start_pos;
    int sabaiba_count = 0;
    int killer_count = 0;
    // Start is called before the first frame update
    [PunRPC]
    void RPC_SetOpen()
    {
       
        sabaiba_count += 1;
        if (sabaiba_count == 2)
        {
            sabaiba_text.text = "サバイバー準備OK";
        }
        else
        {
            sabaiba_text.text = sabaiba_count.ToString();
        }
        if (sabaiba_count == 2 && killer_count == 1)
        {
            GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
            GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
            GameObject.FindWithTag("Killer").transform.position = Start_pos[3].position;
        }
    }
    [PunRPC]
    void RPC_SetOpen2()
    {
      
        sabaiba_count -= 1;
      sabaiba_text.text = sabaiba_count.ToString();
        if (sabaiba_count == 2 && killer_count == 1)
        {
            GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
            GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
            GameObject.FindWithTag("Killer").transform.position = Start_pos[3].position;
        }

    }
    [PunRPC]
    void RPC_SetOpen3()
    {

        killer_count += 1;
        if (killer_count == 1)
        {
            killer_text.text = "キラー準備OK";
        }
        else
        {
            killer_text.text = killer_count.ToString();
        }
        if (sabaiba_count == 2 && killer_count == 1)
        {
            GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
            GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
            GameObject.FindWithTag("Killer").transform.position = Start_pos[3].position;
        }
    }
    [PunRPC]
    void RPC_SetOpen4()
    {

        killer_count -= 1;
        killer_text.text = killer_count.ToString();
        if (sabaiba_count == 2 && killer_count == 1)
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
