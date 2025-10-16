using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] int gameManager;
    [SerializeField] GameObject camera_Object;

    [Header("アウトライン")]
    [SerializeField] Outline outline_Script;
    float side;
    float ver;

    bool Is_PlayMode = false;

    Rigidbody rb;
    Ray ray;
    MainGameManager mainGameManager;
    [SerializeField] GameObject camera;

    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private GameObject rayObject;

    [SerializeField] Text selectTextLabel;

    GameObject[] Barrier_red;
    GameObject[] Barrier_blue;
    string PlayerColor = "";
    string PlayerRoleName = "";
    private Outline _outline;
    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
        if (_outline == null)
        {
            Debug.LogError("Outline component not found on this player.");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
            Is_PlayMode = true;
            // 自分自身のUIのみを検索する
            if (photonView.IsMine)
            {
                selectTextLabel = GameObject.FindGameObjectWithTag("selectUI").GetComponent<Text>();
            }
            mainGameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MainGameManager>();
        }


        if (photonView.IsMine && Is_PlayMode)
        {
            StartCoroutine(Test());
        }
    }

    System.Collections.IEnumerator Test()
    {
        yield return new WaitForSeconds(1.0f);

        Barrier_red = GameObject.FindGameObjectsWithTag("red");
        Barrier_blue = GameObject.FindGameObjectsWithTag("blue");
        PlayerRoleName = mainGameManager.TrySetRoleLabel(PhotonNetwork.LocalPlayer);
        rb = GetComponent<Rigidbody>();

        // 初期色の設定
        object colorValue;
        if (photonView.Owner.CustomProperties.TryGetValue("Color", out colorValue))
        {
            string initialColor = mainGameManager.Get_ColorType(PhotonNetwork.LocalPlayer);
            if (!string.IsNullOrEmpty(initialColor))
            {
                PlayerColor = initialColor;
            }
        }

    }
    // Update is called once per frame

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKey("w"))
            {
                transform.position += transform.forward * 5 * Time.deltaTime;

            }
            if (Input.GetKey("s"))
            {
                transform.position -= transform.forward * 5 * Time.deltaTime;

            }
            if (Input.GetKey("a"))
            {
                transform.position -= transform.right * 5 * Time.deltaTime;
            }
            if (Input.GetKey("d"))
            {
                transform.position += transform.right * 5 * Time.deltaTime;
            }
        }
    }
    void Update()
    {
        // Ray作成
        ray = new Ray(rayObject.transform.position, rayObject.transform.forward);
        // Sceneビューで赤い線を描画

        if (photonView.IsMine)
        {
            ApplyOutlineColor(PlayerColor);

            //Rayのエラーのため、無効化
            //photonView.RPC("SetRay", RpcTarget.AllBuffered);
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");
            side += h;
            ver += v;
            ver = Mathf.Clamp(ver, -90f, 90f);
            transform.rotation = Quaternion.Euler(0f, side, 0f);
            camera.transform.rotation = Quaternion.Euler(-ver, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z);
            if (gameObject.layer == 7)
            {
                // gameObject.tag = ("Killer");
            }
            else if (gameObject.layer == 6)
            {
                // gameObject.tag = ("Player");
            }

            //if (this.gameObject.tag == "Killer")
            //{
            //    GameObject.Find("killerImage").SetActive(true);
            //    GameObject.Find("PlayerImage").SetActive(false);
            //    GameObject.Find("Player2Image").SetActive(false);
            //}
            //else if (this.gameObject.tag == "Player")
            //{
            //    GameObject.Find("killerImage").SetActive(false);
            //    GameObject.Find("PlayerImage").SetActive(true);
            //    GameObject.Find("Player2Image").SetActive(false);
            //}
            //else if (this.gameObject.tag == "Player2")
            //{
            //    GameObject.Find("killerImage").SetActive(false);
            //    GameObject.Find("PlayerImage").SetActive(false);
            //    GameObject.Find("Player2Image").SetActive(true);
            //}


            RaycastHit hit;

            // Ray飛ばす
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("DOA"))
                {

                    Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                    if (animator.GetBool("open") == false)
                    {
                        selectTextLabel.text = "[F]開ける";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", true);
                        }
                    }
                    else if (animator.GetBool("open") == true)
                    {
                        selectTextLabel.text = "[F]閉める";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", false);
                        }
                    }


                }
                else if (hit.collider.CompareTag("Shelf"))
                {

                    Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                    if (animator.GetBool("open") == false)
                    {
                        selectTextLabel.text = "[F]開ける";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", true);
                        }
                    }
                    else if (animator.GetBool("open") == true)
                    {
                        selectTextLabel.text = "[F]閉める";
                        if (Input.GetKeyDown("f"))
                        {
                            animator.SetBool("open", false);
                        }
                    }


                }
                else if (Is_PlayMode)
                {
                    // DOAじゃないオブジェクトに当たった時は非表示
                    selectTextLabel.text = "";
                }
            }
            else if (Is_PlayMode)
            {
                // 何にも当たらなかったら非表示
                selectTextLabel.text = "";
            }

            if (Input.GetKeyDown("c") && Is_PlayMode && PlayerRoleName != "killer")
            {
                string newColor = PlayerColor == "red" ? "blue" : "red";

                // 自分の色を変更
                mainGameManager.ChangeColor(newColor);
            }

        }
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.red);

        if (Is_PlayMode && photonView.IsMine)
        {

            Barrier_Collider();
        }


        //error fix
        //camera_Object.transform.rotation = Quaternion.Euler(-ver, transform.eulerAngles.y, 0f);
    }

    void Barrier_Collider()
    {
        if (Barrier_red == null || Barrier_blue == null || PlayerRoleName == "killer") return;

        // PlayerColorはOnPlayerPropertiesUpdateで更新されているので、それを直接使う
        bool isRed = (PlayerColor == "red");

        foreach (GameObject obj in Barrier_red)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null) col.enabled = !isRed; // 赤なら無効、青なら有効
        }
        foreach (GameObject obj in Barrier_blue)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null) col.enabled = isRed; // 赤なら有効、青なら無効
        }
    }

    private void ApplyOutlineColor(string colorStr)
    {
        if (_outline == null || _outline.outlineFillMaterial == null) return;

        Color c = Color.clear;
        if (colorStr == "red") c = Color.red;
        else if (colorStr == "blue") c = Color.blue;

        _outline.outlineFillMaterial.SetColor("_OutlineColor", c);
        Debug.Log($"[Outline] {photonView.Owner.NickName} => {colorStr}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("再同期開始...");
        if (photonView.IsMine)
        {
            PlayerColor = mainGameManager.Get_ColorType(PhotonNetwork.LocalPlayer);
            ApplyOutlineColor(PlayerColor);
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!changedProps.ContainsKey("Color")) return;

        string newColor = (string)changedProps["Color"];
        if (targetPlayer == photonView.Owner)
        {
            PlayerColor = newColor;
            ApplyOutlineColor(PlayerColor);
            Debug.Log($"[Sync] {targetPlayer.NickName} の色更新: {newColor}");
        }
    }
}
