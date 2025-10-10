using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class RollDecider : MonoBehaviour
{
    [SerializeField] Button survivorButton;
    [SerializeField] Button killerButton;
    [SerializeField] GameObject SelectButtons;

    void Start()
    {
        survivorButton.onClick.AddListener(() => SelectRole("survivor"));
        killerButton.onClick.AddListener(() => SelectRole("killer"));
    }

    void SelectRole(string role)
    {
        // 希望ロールを CustomProperties に保存
        Hashtable props = new Hashtable();
        props["RequestedRole"] = role;
        props["IsReady"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        Debug.Log($"希望ロールを選択: {role}");
        SelectButtons.SetActive(false);
    }

    public void Disable_SelectButtons()
    {
        SelectButtons.SetActive(false);
    }
}
