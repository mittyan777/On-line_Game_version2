using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameStart_count : MonoBehaviourPun
{
    [SerializeField] Text sabaiba_text;
    [SerializeField] Text killer_text;
    [SerializeField] Text count_text;
    [SerializeField] float count = 5;
    int result;
    [SerializeField] Transform[] Start_pos;
    [SerializeField]GameObject Gamemanager;
    bool time_count_trigger = false;
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
            sabaiba_text.text = ($"{Gamemanager.GetComponent<MainGameManager>().sabaiba_count}/ 2");
        }
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            sabaiba_text.gameObject.SetActive(false);
            killer_text.gameObject.SetActive(false);
            count_text.gameObject.SetActive(true);
            time_count_trigger = true;
        }
    }
    [PunRPC]
    void RPC_SetOpen2()
    {

        Gamemanager.GetComponent<MainGameManager>().sabaiba_count -= 1;
      sabaiba_text.text = ($"{Gamemanager.GetComponent<MainGameManager>().sabaiba_count}/2");
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            sabaiba_text.gameObject.SetActive(false);
            killer_text.gameObject.SetActive(false);
            count_text.gameObject.SetActive(true);
            time_count_trigger = true;
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
            killer_text.text = ($"{Gamemanager.GetComponent<MainGameManager>().killer_count}/1");
        }
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            sabaiba_text.gameObject.SetActive(false);
            killer_text.gameObject.SetActive(false);
            count_text.gameObject.SetActive(true);
            time_count_trigger = true;
        }
    }
    [PunRPC]
    void RPC_SetOpen4()
    {

        Gamemanager.GetComponent<MainGameManager>().killer_count -= 1;
        killer_text.text = ($"{Gamemanager.GetComponent<MainGameManager>().killer_count}/1");
        if (Gamemanager.GetComponent<MainGameManager>().sabaiba_count == 2 && Gamemanager.GetComponent<MainGameManager>().killer_count == 1)
        {
            sabaiba_text.gameObject.SetActive(false);
            killer_text.gameObject.SetActive(false);
            count_text.gameObject.SetActive(true);
            time_count_trigger = true;
            
        }

    }
    private void Update()
    {
       if(time_count_trigger == true)
       {
            int result = Mathf.FloorToInt(count);
            count -= Time.deltaTime;
            count_text.text = ($"{result}");
       }
       if(count <= 0)
       {
            if (Gamemanager.GetComponent<MainGameManager>().GameStart_trigger == false)
            {
                GameObject.FindWithTag("Player").transform.position = Start_pos[0].position;
                GameObject.FindWithTag("Player2").transform.position = Start_pos[1].position;
                GameObject.FindWithTag("Killer").transform.position = Start_pos[2].position;
            }
            count_text.gameObject.SetActive(false);
            Gamemanager.GetComponent<MainGameManager>().GameStart_trigger = true;
       }
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
