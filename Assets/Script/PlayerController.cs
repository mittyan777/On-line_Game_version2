using Photon.Pun;
using System.Collections;
using TMPro;
using UniGLTF.Extensions.VRMC_vrm;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniVRM10;
using static UnityEngine.GraphicsBuffer;


public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] int gameManager;
    [SerializeField] GameObject camera_Object;
    [SerializeField] Canvas playerCanvas;
    [Header("アウトライン")]
    [SerializeField] Outline outline_Script;
    float side;
    float ver;
    float MoveSpeed = 5;

    bool Is_PlayMode = false;

    Rigidbody rb;
    Ray ray;
    [SerializeField] GameObject camera;

    [SerializeField] private float rayDistance = 0.01f;
    [SerializeField] private GameObject rayObject;

    [SerializeField] Text select;
    string collar = "";
    [SerializeField] private GameObject Manager;
    GameObject OFObject;
    [SerializeField] Animator animator;

    [SerializeField] GameObject passwordUI;
    [SerializeField] GameObject[] memo;
    [SerializeField] GameObject Notepad;
    [SerializeField] public GameObject Mermaid;
    [SerializeField] public GameObject killer_skin;
    [SerializeField] public GameObject Ghost_skin;

    [Header("アウトライン能力")]
    [SerializeField] float skillDuration = 5f; // 壁越しアウトラインの持続時間

    private int normalLayer;
    private int outlineLayer;

    [SerializeField] GameObject itemslot;
    [SerializeField] GameObject skillslot;
    [SerializeField] GameObject cooltime_Image;
    public bool Trap_trigger = false;
    GameObject tora;
    [SerializeField] GameObject Drone_Player_Detection;
    [SerializeField] Image fade;
    public bool fade_trigger = false;
    float fade_a;

    [SerializeField] GameObject tutorial_UI;

    [SerializeField] string detainee_name;
    string Back_name;

    public ExpressionPreset preset;

    [SerializeField] GameObject Record_name;
    [SerializeField] public TextMeshPro my_name;


    // Start is called before the first frame update
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
            Is_PlayMode = true;
            normalLayer = LayerMask.NameToLayer("PlayerNormal");
            outlineLayer = LayerMask.NameToLayer("OutlineVisible");
            fade = GameObject.FindWithTag("fade").GetComponent<Image>();
            cooltime_Image.GetComponent<Image>().fillAmount = 0;
            MainGameManager.Player_name = new string[3];
            //Roll Check
            Invoke(nameof(PlayerStart), 5);
        }
        if (sceneName == "lobby")
        {
            Record_name.SetActive(false);
        }

        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    void PlayerStart()
    {
        if (!photonView.IsMine)
        {
            playerCanvas.gameObject.SetActive(false);
        }

        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            string role = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];

            // 全員に共有する
            photonView.RPC("SetRole", RpcTarget.AllBuffered, role);

            Record_name.SetActive(true);
            if (this.gameObject.tag == "Killer")
            {
                GameObject.Find("killerImage").SetActive(true);
                GameObject.Find("PlayerImage").SetActive(false);
                GameObject.Find("Player2Image").SetActive(false);
                Manager.GetComponent<MainGameManager>().killer_skin(gameObject.tag);
                camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("killer_skin"));
            }
            else if (this.gameObject.tag == "Player")
            {
                GameObject.Find("killerImage").SetActive(false);
                GameObject.Find("PlayerImage").SetActive(true);
                GameObject.Find("Player2Image").SetActive(false);
                camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Ghost_skin"));
            }
            else if (this.gameObject.tag == "Player2")
            {
                GameObject.Find("killerImage").SetActive(false);
                GameObject.Find("PlayerImage").SetActive(false);
                GameObject.Find("Player2Image").SetActive(true);
                camera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("Ghost_skin"));
            }
            passwordUI = GameObject.Find("InputField");
            passwordUI.SetActive(false);
        }

    }

    [PunRPC]
    void SetRole(string role)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            if (role == "killer")
            {
                gameObject.layer = LayerMask.NameToLayer("Killer");
                gameObject.tag = "Killer";
                itemslot.SetActive(false);
                skillslot.SetActive(true);
                Drone_Player_Detection.SetActive(false);
                Debug.Log("あなたは Killer です！");
            }
            else if (role == "survivor")
            {
                skillslot.SetActive(false);
                itemslot.SetActive(true);
                gameObject.layer = LayerMask.NameToLayer("Player");
                if (players.Length == 0)
                {
                    gameObject.tag = "Player";

                }
                else if (players.Length == 1)
                {
                    gameObject.tag = "Player2";

                }

                Debug.Log("あなたは Survivor です！");
            }
            else
            {
                Debug.Log("ロールが設定されていません。");
            }
        }
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (photonView.IsMine && sceneName == "main")
        {
            if (passwordUI != null)
            {
                if (passwordUI.activeSelf == false)
                {
                    float x = 0f;
                    float z = 0f;
                    if (Trap_trigger == false)
                    {
                        if (Input.GetKey("w"))
                        {
                            z += 1f;
                            animator.SetBool("forward_walk", true);
                        }
                        else animator.SetBool("forward_walk", false);

                        if (Input.GetKey("s"))
                        {
                            z -= 1f;
                            animator.SetBool("back_walk", true);
                        }
                        else animator.SetBool("back_walk", false);

                        if (Input.GetKey("a"))
                        {
                            x -= 1f;
                            animator.SetBool("left_walk", true);
                        }
                        else animator.SetBool("left_walk", false);

                        if (Input.GetKey("d"))
                        {
                            x += 1f;
                            animator.SetBool("right_walk", true);
                        }
                        else animator.SetBool("right_walk", false);

                        if (Input.GetKeyDown(KeyCode.Q))
                        {
                            if (Notepad.activeSelf == false)
                            {
                                Notepad.SetActive(true);
                            }
                            else if (Notepad.activeSelf == true)
                            {
                                Notepad.SetActive(false);
                            }
                        }

                        //アウトライン透過スキル　発動
                        if (gameObject.tag == "Killer" && Input.GetKey("e"))
                        {
                            if (cooltime_Image.GetComponent<Image>().fillAmount <= 0)
                            {
                                cooltime_Image.GetComponent<Image>().fillAmount = 1;
                                Manager.GetComponent<MainGameManager>().killer_skill();

                            }

                        }

                        if (Input.GetKey(KeyCode.Alpha1))
                        {
                            Manager.GetComponent<MainGameManager>().Face_swap(gameObject.tag);
                        }
                        else
                        {
                            Manager.GetComponent<MainGameManager>().Face_swapOF(gameObject.tag);
                        }


                        Vector3 move = (transform.forward * z + transform.right * x).normalized;

                        Vector3 velocity = move * MoveSpeed;
                        velocity.y = rb.velocity.y; // 重力維持

                        rb.velocity = velocity;
                    }
                }
            }
            cooltime_Image.GetComponent<Image>().fillAmount -= 0.005f * Time.deltaTime;
            if (gameObject.tag == "Killer")
            {
                if (cooltime_Image.GetComponent<Image>().fillAmount <= 0.95f)
                {
                    Manager.GetComponent<MainGameManager>().killer_skillOF();
                }
            }

            if (Input.GetKeyDown("h"))
            {
                Manager.GetComponent<MainGameManager>().Game_Clear(gameObject.tag);
                //Manager.GetComponent<MainGameManager>().Game_over();
            }

        }
        if (photonView.IsMine && sceneName == "lobby")
        {
            float x = 0f;
            float z = 0f;

            if (Input.GetKey("w"))
            {
                z += 1f;
                animator.SetBool("forward_walk", true);
            }
            else animator.SetBool("forward_walk", false);

            if (Input.GetKey("s"))
            {
                z -= 1f;
                animator.SetBool("back_walk", true);
            }
            else animator.SetBool("back_walk", false);

            if (Input.GetKey("a"))
            {
                x -= 1f;
                animator.SetBool("left_walk", true);
            }
            else animator.SetBool("left_walk", false);

            if (Input.GetKey("d"))
            {
                x += 1f;
                animator.SetBool("right_walk", true);
            }
            else animator.SetBool("right_walk", false);

            Vector3 move = (transform.forward * z + transform.right * x).normalized;

            Vector3 velocity = move * MoveSpeed;
            velocity.y = rb.velocity.y; // 重力維持

            rb.velocity = velocity;
        }
    }
    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
            if (GameObject.Find("GameManager") != null)
            {
                Manager = GameObject.Find("GameManager");
            }
            if (select == null)
            {
                select = GameObject.FindGameObjectWithTag("selectUI").GetComponent<Text>();
            }


        }

        // Ray作成
        ray = new Ray(rayObject.transform.position, rayObject.transform.forward);
        // Sceneビューで赤い線を描画

        if (photonView.IsMine)
        {
            //playerCanvas.enabled = true;
            //Rayのエラーのため、無効化
            //photonView.RPC("SetRay", RpcTarget.AllBuffered);

            if (sceneName == "main")
            {
                if (passwordUI != null)
                {
                    if (passwordUI.activeSelf == false)
                    {


                        float h = Input.GetAxis("Mouse X");
                        float v = Input.GetAxis("Mouse Y");
                        side += h;
                        ver += v;
                        ver = Mathf.Clamp(ver, -50f, 90f);
                        // side = Mathf.Clamp(side, -90, 90f);
                        camera.transform.rotation = Quaternion.Euler(-ver, side, camera.transform.eulerAngles.z);

                        transform.rotation = Quaternion.Euler(0f, side, 0f);

                    }
                }
            }
            if (sceneName == "lobby")
            {
                float h = Input.GetAxis("Mouse X");
                float v = Input.GetAxis("Mouse Y");
                side += h;
                ver += v;
                ver = Mathf.Clamp(ver, -50f, 90f);
                // side = Mathf.Clamp(side, -90, 90f);
                camera.transform.rotation = Quaternion.Euler(-ver, side, camera.transform.eulerAngles.z);

                transform.rotation = Quaternion.Euler(0f, side, 0f);
            }

            if (sceneName == "main")
            {
                RaycastHit hit;

                // Ray飛ばす
                if (Physics.Raycast(ray, out hit, rayDistance))
                {
                    Debug.Log("Ray hit: " + hit.collider.name + " Tag: " + hit.collider.tag);
                    if (hit.collider.CompareTag("DOA"))
                    {

                        Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                        if (animator.GetBool("open") == false)
                        {
                            select.text = "[F]開ける";
                            if (Input.GetKeyDown("f"))
                            {
                                DoorController door = hit.collider.GetComponent<DoorController>();
                                if (door != null)
                                {
                                    door.SetOpen(true);
                                }
                            }
                        }
                        else if (animator.GetBool("open") == true)
                        {
                            select.text = "[F]閉める";
                            if (Input.GetKeyDown("f"))
                            {
                                DoorController door = hit.collider.GetComponent<DoorController>();
                                if (door != null)
                                {
                                    door.SetOpen(false);
                                }
                            }
                        }


                    }
                    else if (hit.collider.CompareTag("Shelf"))
                    {

                        Animator animator = hit.collider.transform.parent.GetComponent<Animator>();
                        if (animator.GetBool("open") == false)
                        {
                            select.text = "[F]開ける";
                            if (Input.GetKeyDown("f"))
                            {
                                DoorController door = hit.collider.transform.parent.GetComponent<DoorController>();
                                if (door != null)
                                {
                                    door.SetOpen(true);
                                }
                            }
                        }
                        else if (animator.GetBool("open") == true)
                        {
                            select.text = "[F]閉める";
                            if (Input.GetKeyDown("f"))
                            {
                                DoorController door = hit.collider.transform.parent.GetComponent<DoorController>();
                                if (door != null)
                                {
                                    door.SetOpen(false);
                                }
                            }
                        }


                    }
                    else if (hit.collider.CompareTag("tora"))
                    {
                        select.text = "[F]回収";
                        if (Input.GetKeyDown("f") && GetComponent<ItemSelect>().tora == false)
                        {
                            GetComponent<ItemSelect>().tora = true;

                            photonView.RPC("DestroyObject", RpcTarget.MasterClient, hit.collider.gameObject.GetComponent<PhotonView>().ViewID);

                        }
                    }
                    else if (hit.collider.CompareTag("Ext_Door"))
                    {

                        Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                        if (hit.collider.gameObject.GetComponent<Exit>().rock == true)
                        {
                            select.text = "[F]パスコードを入力する";
                            if (Input.GetKeyDown("f"))
                            {
                                passwordUI.SetActive(true);
                            }

                        }
                        else
                        {
                            passwordUI.SetActive(false);
                            if (animator.GetBool("open") == false)
                            {
                                select.text = "[F]開ける";
                                if (Input.GetKeyDown("f"))
                                {
                                    DoorController door = hit.collider.GetComponent<DoorController>();
                                    if (door != null)
                                    {
                                        door.SetOpen(true);
                                    }
                                }



                            }
                        }

                    }
                    else if (hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "Player2")
                    {
                        if (gameObject.tag == "Killer")
                        {
                            select.text = "[F]捕まえる";
                            if (Input.GetKeyDown("f"))
                            {

                                Manager.GetComponent<MainGameManager>().killer_Securing(hit.collider.gameObject.tag);

                            }
                        }
                    }


                    else if (hit.collider.CompareTag("Prison_door"))
                    {
                        Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                        if (animator.GetBool("open") == false)
                        {
                            if (gameObject.tag != detainee_name)
                            {
                                select.text = "[F]開ける";
                                if (Input.GetKeyDown("f"))
                                {
                                    DoorController door = hit.collider.GetComponent<DoorController>();
                                    if (door != null)
                                    {
                                        door.SetOpen(true);
                                    }
                                }
                            }
                        }
                        else if (animator.GetBool("open") == true)
                        {
                            select.text = "[F]閉める";
                            if (Input.GetKeyDown("f"))
                            {
                                DoorController door = hit.collider.GetComponent<DoorController>();
                                if (door != null)
                                {
                                    door.SetOpen(false);
                                }
                            }
                        }




                    }
                    else if (hit.collider.name == ("number1"))
                    {
                        memo[0].GetComponent<Text>().text = hit.collider.GetComponent<Text>().text;
                    }
                    else if (hit.collider.name == ("number2"))
                    {
                        memo[1].GetComponent<Text>().text = hit.collider.GetComponent<Text>().text;
                    }
                    else if (hit.collider.name == ("number3"))
                    {
                        memo[2].GetComponent<Text>().text = hit.collider.GetComponent<Text>().text;
                    }
                    else if (hit.collider.name == ("number4"))
                    {
                        memo[3].GetComponent<Text>().text = hit.collider.GetComponent<Text>().text;
                    }
                    else
                    {
                        // DOAじゃないオブジェクトに当たった時は非表示
                        select.text = "";
                    }
                }
                else if (Is_PlayMode)
                {
                    // 何にも当たらなかったら非表示
                    select.text = "";
                }

                if (fade_trigger == true && fade.fillAmount <= 1)
                {
                    fade.fillAmount += 2 * Time.deltaTime;

                }
                else if (fade.fillAmount >= 0)
                {
                    fade.fillAmount -= 2 * Time.deltaTime;
                }

            }
            if (gameObject.tag != "Killer")
            {

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
                {
                    MoveSpeed = 8;
                    animator.SetBool("dash", true);
                    if (camera.GetComponent<Camera>().fieldOfView < 80)
                    {
                        camera.GetComponent<Camera>().fieldOfView += 80f * Time.deltaTime;
                    }
                }
                else
                {
                    animator.SetBool("dash", false);
                    MoveSpeed = 5;
                    if (camera.GetComponent<Camera>().fieldOfView > 60)
                    {
                        camera.GetComponent<Camera>().fieldOfView -= 50f * Time.deltaTime;
                    }
                }
            }
        }
        else
        {
            //playerCanvas.enabled = false;
        }
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.red);

        //error fix
        //camera_Object.transform.rotation = Quaternion.Euler(-ver, transform.eulerAngles.y, 0f);
    }



    void ApplyIfOtherPlayer(GameObject obj)
    {
        PhotonView pv = obj.GetComponent<PhotonView>();
        if (pv != null && pv.Owner != photonView.Owner)
        {
            StartCoroutine(ShowOutlineThroughWalls(obj, skillDuration));
        }
    }

    IEnumerator ShowOutlineThroughWalls(GameObject targetPlayer, float duration)
    {
        // 外見モデルのみをレイヤー変更
        SetMeshLayer(targetPlayer, outlineLayer);

        yield return new WaitForSeconds(duration);

        SetMeshLayer(targetPlayer, normalLayer);
    }

    void SetMeshLayer(GameObject obj, int layer)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.gameObject.layer = layer;
        }
    }

    [PunRPC]
    void DestroyObject(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;
        if (gameObject.tag == "Killer")
        {
            if (other.gameObject.tag == "bear trap")
            {
                tora = other.gameObject;
                Debug.Log("iii");
                StartCoroutine(Trap());
            }
        }
        if (other.gameObject.tag == "tutorial")
        {
            tutorial_UI.SetActive(true);
            tutorial_UI.GetComponent<Animator>().SetBool("show", true);
        }
        if (other.gameObject.name == "Player_count")
        {
            GameObject.FindWithTag("Player_count").gameObject.GetComponent<GameStart_count>().SetOpen();
        }
        if (other.gameObject.name == "killer_count")
        {
            GameObject.FindWithTag("Player_count").gameObject.GetComponent<GameStart_count>().SetOpen3();
        }
        if (other.gameObject.name == "END")
        {
            StartCoroutine(END());
        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player_count")
        {
            GameObject.FindWithTag("Player_count").gameObject.GetComponent<GameStart_count>().SetOpen2();
        }
        if (other.gameObject.name == "killer_count")
        {
            GameObject.FindWithTag("Player_count").gameObject.GetComponent<GameStart_count>().SetOpen4();
        }
        if (gameObject.name == "Jail")
        {
            detainee_name = "";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Jail")
        {
            //Manager.GetComponent<MainGameManager>().Game_over();
            detainee_name = gameObject.tag;
            Back_name = collision.gameObject.name;
            collision.gameObject.name = "Jail_Player";
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (gameObject.name == "Jail")
        {
            Manager.GetComponent<MainGameManager>().Game_over_of();
            collision.gameObject.name = Back_name;
        }
    }
    IEnumerator Trap()
    {
        Trap_trigger = true;
        yield return new WaitForSeconds(5f);
        Destroy(tora);
        Trap_trigger = false;
    }

    IEnumerator END()
    {
        fade_trigger = true;
        yield return new WaitForSeconds(4f);
        Manager.GetComponent<MainGameManager>().Game_Clear(gameObject.tag);
        fade_trigger = false;
        //PhotonResetManager.Instance.BackToTitle();

    }
    IEnumerator tutorial()
    {
        tutorial_UI.GetComponent<Animator>().SetBool("tutorial_of", true);
        yield return new WaitForSeconds(3f);
        tutorial_UI.SetActive(false);
    }
    public void tutorial_of()
    {
        StartCoroutine(tutorial());
    }
    public void name_Input(string name)
    {
        name = Record_name.GetComponent<TMP_InputField>().text;
        Manager.GetComponent<MainGameManager>().name_Record(name, gameObject.tag);
        Record_name.SetActive(false);
    }


}
