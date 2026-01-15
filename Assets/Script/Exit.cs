using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Exit : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject InputField;
    [SerializeField] GameObject InputField_Child;
    [SerializeField] Text[] number_text;
    [SerializeField] Text message;

    int Exit_number;
    [SerializeField]string numberStr;
    string Input_number;

    public bool rock = true;

   

    void Start()
    {
        // オフライン or ルーム未参加なら何もしない
        if (!PhotonNetwork.InRoom && !PhotonNetwork.OfflineMode)
            return;

        if (PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode)
        {
            int num = Random.Range(1000, 9999);
            photonView.RPC(nameof(RPC_SetExitNumber), RpcTarget.All, num);
        }
    }

    [PunRPC]
    void RPC_SetExitNumber(int num)
    {
        Exit_number = num;
        numberStr = Exit_number.ToString();

        number_text[0].text = numberStr.Substring(0, 1);
        number_text[1].text = numberStr.Substring(1, 1);
        number_text[2].text = numberStr.Substring(2, 1);
        number_text[3].text = numberStr.Substring(3, 1);
    }

    void Update()
    {
        Input_number = InputField_Child.GetComponent<TMP_InputField>().text;
        Debug.Log(numberStr);
    }

    public void Confirmation()
    {
        if (Input_number == numberStr)
        {
            rock = false;
            message.text = "鍵が開きました";
            Invoke(nameof(message_reset), 2);
        }
        else
        {
            message.text = "パスワードが違います";
            Invoke(nameof(message_reset), 2);
        }
    }

    void message_reset()
    {
        message.text = "";
    }

    public void Cancel()
    {
        InputField.SetActive(false);
    }
}
