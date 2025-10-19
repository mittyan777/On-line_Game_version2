using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject Manager;
    private Animator animator;
    private Animator animator2;


    // Start is called before the first frame update
    void Start()
    {
        animator = Playersw.GetComponent<Animator>();
        animator2 = Playersw2.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

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
                if (gameObject.tag == "Player")
                {
                    Playersw.SetActive(true);
                }
                else if (gameObject.tag == "Player2")
                {
                    Playersw2.SetActive(true);
                }
            }
            else
            {
                if (gameObject.tag == "Player")
                {
                    Playersw.SetActive(false);
                }
                else if (gameObject.tag == "Player2")
                {
                    Playersw2.SetActive(false);
                }
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
        Debug.Log($"使用されたアイテムID:{num}");
        if (num == 0)
        {
            if (photonView.IsMine)
            {
                Manager = GameObject.Find("GameManager");
                if (gameObject.tag == "Player")
                {
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
            }
        }
    }
}