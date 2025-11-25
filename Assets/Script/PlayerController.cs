using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using Photon.Realtime;
using Unity.Burst.CompilerServices;
using static Unity.Burst.Intrinsics.X86.Sse4_2;
using Unity.VisualScripting;

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

    [Header("アウトライン能力")]
    [SerializeField] float skillDuration = 5f; // 壁越しアウトラインの持続時間

    private int normalLayer;
    private int outlineLayer;

    [SerializeField] GameObject itemslot;
    [SerializeField] GameObject skillslot;
    [SerializeField] GameObject cooltime_Image;
    bool Trap_trigger = false;
    GameObject tora;
    [SerializeField] GameObject Drone_Player_Detection;
    [SerializeField] Image fade;
    bool fade_trigger = false;
    float fade_a;

    [SerializeField] GameObject tutorial_UI;

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
            //Roll Check
            Invoke("test", 5);
        }

    }

    void test()
    {

        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody>();
            string role = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];

            // 全員に共有する
            photonView.RPC("SetRole", RpcTarget.AllBuffered, role);

            if (this.gameObject.tag == "Killer")
            {
                GameObject.Find("killerImage").SetActive(true);
                GameObject.Find("PlayerImage").SetActive(false);
                GameObject.Find("Player2Image").SetActive(false);
            }
            else if (this.gameObject.tag == "Player")
            {
                GameObject.Find("killerImage").SetActive(false);
                GameObject.Find("PlayerImage").SetActive(true);
                GameObject.Find("Player2Image").SetActive(false);
            }
            else if (this.gameObject.tag == "Player2")
            {
                GameObject.Find("killerImage").SetActive(false);
                GameObject.Find("PlayerImage").SetActive(false);
                GameObject.Find("Player2Image").SetActive(true);
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
                        // 入力取得
                        if (Input.GetKey("w"))
                        {
                            z += 1f;
                            animator.SetBool("forward_walk", true);
                        }
                        else
                        {
                            animator.SetBool("forward_walk", false);
                        }
                        if (Input.GetKey("s"))
                        {
                            z -= 1f;
                            animator.SetBool("back_walk", true);
                        }
                        else
                        {
                            animator.SetBool("back_walk", false);
                        }
                        if (Input.GetKey("a"))
                        {
                            x -= 1f;
                            animator.SetBool("left_walk", true);
                        }
                        else
                        {
                            animator.SetBool("left_walk", false);
                        }
                        if (Input.GetKey("d"))
                        {
                            x += 1f;
                            animator.SetBool("right_walk", true);
                        }
                        else
                        {
                            animator.SetBool("right_walk", false);
                        }

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
                        if (gameObject.tag == "Killer" && Input.GetKeyDown(KeyCode.RightShift))
                        {
                            if (cooltime_Image.GetComponent<Image>().fillAmount <= 0)
                            {
                                photonView.RPC(nameof(RPC_ActivateOutlineSkill), RpcTarget.All);
                                cooltime_Image.GetComponent<Image>().fillAmount = 1;
                            }

                        }

                        cooltime_Image.GetComponent<Image>().fillAmount -= 0.005f * Time.deltaTime;
                        // 移動方向ベクトル
                        Vector3 move = (transform.forward * z + transform.right * x).normalized;

                        // 実際の移動
                        transform.position += move * MoveSpeed * Time.deltaTime;
                    }
                }
            }
        }
        if (photonView.IsMine && sceneName == "lobby")
        {
            float x = 0f;
            float z = 0f;

            // 入力取得
            if (Input.GetKey("w"))
            {
                z += 1f;
                animator.SetBool("forward_walk", true);
            }
            else
            {
                animator.SetBool("forward_walk", false);
            }
            if (Input.GetKey("s"))
            {
                z -= 1f;
                animator.SetBool("back_walk", true);
            }
            else
            {
                animator.SetBool("back_walk", false);
            }
            if (Input.GetKey("a"))
            {
                x -= 1f;
                animator.SetBool("left_walk", true);
            }
            else
            {
                animator.SetBool("left_walk", false);
            }
            if (Input.GetKey("d"))
            {
                x += 1f;
                animator.SetBool("right_walk", true);
            }
            else
            {
                animator.SetBool("right_walk", false);
            }

            // 移動方向ベクトル
            Vector3 move = (transform.forward * z + transform.right * x).normalized;

            // 実際の移動
            transform.position += move * MoveSpeed * Time.deltaTime;
        }
    }
    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "main")
        {
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
            playerCanvas.enabled = true;
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
                    if (hit.collider.CompareTag("DOA"))
                    {

                        Animator animator = hit.collider.gameObject.GetComponent<Animator>();
                        if (animator.GetBool("open") == false)
                        {
                            select.text = "[F]開ける";
                            if (Input.GetKeyDown("f"))
                            {
                                animator.SetBool("open", true);
                            }
                        }
                        else if (animator.GetBool("open") == true)
                        {
                            select.text = "[F]閉める";
                            if (Input.GetKeyDown("f"))
                            {
                                animator.SetBool("open", false);
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
                                animator.SetBool("open", true);
                            }
                        }
                        else if (animator.GetBool("open") == true)
                        {
                            select.text = "[F]閉める";
                            if (Input.GetKeyDown("f"))
                            {
                                animator.SetBool("open", false);
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
                                    animator.SetBool("open", true);
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
        else
        {
            playerCanvas.enabled = false;
        }
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, UnityEngine.Color.red);

        //error fix
        //camera_Object.transform.rotation = Quaternion.Euler(-ver, transform.eulerAngles.y, 0f);
    }

    //アウトライン透過能力
    [PunRPC]
    void RPC_ActivateOutlineSkill()
    {

        // スキル使用者の名前などを出力
        Debug.Log($"{photonView.Owner.NickName} がアウトラインスキルを発動！");

        // 自分以外のプレイヤーを対象に壁越し可視化
        // 全てのプレイヤーオブジェクトを捜索

        GameObject.FindGameObjectWithTag("Player").GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
        GameObject.FindGameObjectWithTag("Player2").GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
        Invoke("ActivateOutlineSkillOF", 5);





    }
    void ActivateOutlineSkillOF()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
        GameObject.FindGameObjectWithTag("Player2").GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
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

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.tag == "Player" || gameObject.tag == "Player2")
        {
            if (collision.gameObject.tag == "Killer")
            {

                StartCoroutine(Trap());
                StartCoroutine(caught());


            }
        }
    }
    IEnumerator Trap()
    {
        Trap_trigger = true;
        yield return new WaitForSeconds(5f);
        Destroy(tora);
        Trap_trigger = false;
    }
    IEnumerator caught()
    {
        fade_trigger = true;
        yield return new WaitForSeconds(3f);
        transform.position = new Vector3(84, 7, 16);
        fade_trigger = false;
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

}
