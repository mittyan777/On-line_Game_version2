using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviourPunCallbacks
{
    public GameObject SelectingSlot;
    public Transform[] ItemSlots;
    public Image[] ItemImages;
    public GameObject[] HavingItems;
    int Current_ItemNum;
    const int MAX_ITEMSLOTS = 3;
    [SerializeField] GameObject Playersw;
    [SerializeField] GameObject Playersw2;
    [SerializeField] GameObject Stop_device;
    [SerializeField] GameObject torabasami;
    [SerializeField] GameObject Installation_clamp;
    [SerializeField] GameObject tora_Installation_position;

    [SerializeField] private GameObject Manager;
    private Animator animator;
    private Animator animator2;
    public bool tora = false;
    [SerializeField] Image Stop_device_cooltime;
    Collar collar = new Collar();
    [SerializeField] GameObject Stop_effect;
    [SerializeField]AudioSource switch_audio;
    // Start is called before the first frame update
    public class Collar
    {
        public UnityEngine.Color color;
    }
    void Start()
    {
        animator = Playersw.GetComponent<Animator>();
        animator2 = Playersw2.GetComponent<Animator>();
        collar.color = ItemSlots[1].GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        Stop_device_cooltime.fillAmount -= Time.deltaTime / 8;    

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(gameObject.tag == "killer")
        {
            SelectingSlot.SetActive(false);
        }
        if (scroll > 0f)
        {
            Debug.Log("ホイール上");
            if (Current_ItemNum <= 0)
            {
                Current_ItemNum = MAX_ITEMSLOTS - 1;
            }
            else
            {
                Current_ItemNum--;
            }
        }
        else if (scroll < 0f)
        {
            Debug.Log("ホイール下");
            if (Current_ItemNum >= MAX_ITEMSLOTS - 1)
            {
                Current_ItemNum = 0;
            }
            else
            {
                Current_ItemNum++;
            }
        }
        if (photonView.IsMine)
        {
            if (Current_ItemNum == 0)
            {
                torabasami.SetActive(false);
                Stop_device.SetActive(false);
                tora_Installation_position.SetActive(false);
                if (gameObject.tag == "Player")
                {
                    Playersw.SetActive(true);
                }
                else if (gameObject.tag == "Player2")
                {
                    Playersw2.SetActive(true);
                }
            }
            else if (Current_ItemNum == 1)
            {
                Playersw.SetActive(false);
                Playersw2.SetActive(false);
                Stop_device.SetActive(false);
                if (gameObject.tag == "Player")
                {
                    if (tora == true)
                    {
                        torabasami.SetActive(true);
                        tora_Installation_position.SetActive(true);
                    }
                    else
                    {
                        tora_Installation_position.SetActive(false);
                    }
                }
                else if (gameObject.tag == "Player2")
                {
                    if (tora == true)
                    {
                        torabasami.SetActive(true);
                        tora_Installation_position.SetActive(true);
                    }
                    else
                    {
                        tora_Installation_position.SetActive(false);
                    }
                }
            }
            else if (Current_ItemNum == 2)
            {
                Playersw.SetActive(false);
                Playersw2.SetActive(false);
                torabasami.SetActive(false);
                Stop_device.SetActive(true);
                tora_Installation_position.SetActive(false);
             
            }
            if (tora == true)
            {
                ItemSlots[1].GetComponent<Image>().sprite = ItemImages[1].sprite;
                ItemSlots[1].GetComponent<Image>().color = UnityEngine.Color.white;
            }

            if (Stop_effect.transform.localScale == new Vector3(1.5f, 1.5f, 1.5f))
            {
                photonView.RPC(nameof(RPCstop), RpcTarget.All);
            }

        }

        // --- ここで null チェック ---
        if (SelectingSlot == null)
        {
            Debug.LogError("SelectingSlot が設定されていません！");
            return;
        }

        if (ItemSlots == null || ItemSlots.Length == 0)
        {
            Debug.LogError("ItemSlots が設定されていません！");
            return;
        }

        if (ItemSlots[Current_ItemNum] == null)
        {
            Debug.LogError($"ItemSlots[{Current_ItemNum}] が null です！");
            return;
        }
        SelectingSlot.transform.position = ItemSlots[Current_ItemNum].position;

        if (Input.GetKeyDown(KeyCode.E)) { UsingItem(Current_ItemNum); }
    }

    void UsingItem(int num)
    {
        if (!photonView.IsMine) return;
        Debug.Log($"使用されたアイテムID:{num}");
        Manager = GameObject.Find("GameManager");
        switch (num)
        {
            case 0:
                //バリア色変更
                if (gameObject.tag == "Player")
                {
                    switch_audio.Play();
                    Manager.GetComponent<MainGameManager>().playercontrol();
                    if (Manager.GetComponent<MainGameManager>().blue == true)
                    {
                        animator.SetBool("switch", true);
                    }
                    if (Manager.GetComponent<MainGameManager>().blue == false)
                    {
                        animator.SetBool("switch", false);
                    }
                }
                if (gameObject.tag == "Player2")
                {
                    switch_audio.Play();
                    Manager.GetComponent<MainGameManager>().player2control();
                    if (Manager.GetComponent<MainGameManager>().red == true)
                    {
                        animator2.SetBool("switch", true);
                    }
                    if (Manager.GetComponent<MainGameManager>().red == false)
                    {
                        animator2.SetBool("switch", false);
                    }
                }
                break;
            case 1:
                //トラばさみ
                if (tora == true)
                {
                    ItemSlots[1].GetComponent<Image>().sprite = null;
                    ItemSlots[1].GetComponent<Image>().color = collar.color;
                    torabasami.SetActive(false);
                    tora = false;
                    PhotonNetwork.Instantiate("ToraPrefab", tora_Installation_position.transform.position, Quaternion.identity);
                }
                break;
            case 2:
                //ドローン停止
                if (Stop_device_cooltime.fillAmount <= 0)
                {
                    switch_audio.Play();
                    photonView.RPC(nameof(RPCPlay), RpcTarget.All);
                    Manager.GetComponent<MainGameManager>().StopDrone_Ability();
                    Stop_device_cooltime.fillAmount = 1f;
                }
                break;
        }
    }
    [PunRPC]
    void RPCPlay()
    {
        Stop_effect.SetActive(true);
    }
    [PunRPC]
    void RPCstop()
    {
        Stop_effect.SetActive(false);
    }
}