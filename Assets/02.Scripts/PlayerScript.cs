using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    //public Rigidbody2D RB;
    public Rigidbody2D RB;

    //public Animator AN;
    public SpriteRenderer SR;
    public PhotonView PV;
    //public Text NickNameText;
    public TMP_Text NickNameText;
    public Image MP;

    public Vector3 v3;

    public float Speed = 3.0f;
    public int spling_power = 10;
    public int banana_power = 400;
    public int Accele_power = 400;

    bool isGround;
    bool curveWall;
    public float Jump_Power = 250f;
    public float hitPower = 3500f;
    int jumpCount;
    float fallSpeed;

    public int count_A = 0;
    public int count_D = 0;

    Vector3 prevPos;
    Vector3 curPos;
    Vector3 lerpPos;

    AudioSource audioSource;

    public AudioClip audioRun;
    public AudioClip audioFart;
    public AudioClip audioVictory;
    public AudioClip audioDefeat;
    public AudioClip audioCrush;
    public AudioClip audioSlip;
    public AudioClip audioDrink;

    void Awake()
    {
        //닉네임
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;
        jumpCount = 1;

        if (PV.IsMine) {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }

        this.audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        

        if (PV.IsMine)
        {
            //방향키 이동
            float axis = Input.GetAxisRaw("Horizontal");
            if (axis < 0)
            {
                SR.flipX = true;
            }
            else if (axis > 0)
            {
                SR.flipX = false;
            }
            
            fallSpeed = RB.velocity.y; // 매번 업데이트 해야하나?
            
            if (isGround)
            {
                if (!(axis == 0f))
                {
                    Vector3 velocity = new Vector3(axis, 0, 0);
                    velocity *= Speed;
                    velocity.y = fallSpeed;
                    RB.velocity = velocity;
                }
                else {
                    RB.velocity *= 0.99f;
                }
            }
            if (count_D >= 5)
            {
                CancelInvoke("WakeUp_D");
            }
            if (count_A >= 5)
            {
                CancelInvoke("WakeUp_A");
            }

            
            //RB.velocity = new Vector2(3 * axis, RB.velocity.y);

            //RB.AddForce(Vector2.right * axis, ForceMode2D.Impulse);


            if (axis != 0)
            {
                //AN.SetBool("walk", true);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);
                PhotonNetwork.SendAllOutgoingCommands();
            }
            else
            {
                //AN.SetBool("walk", false);
            }

            //점프와 바닥체크
            //isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
            //AN.SetBool("jump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround)
            {
                PV.RPC("JumpRPC", RpcTarget.All);
                Debug.Log("점프");
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && isGround)
            {
                PV.RPC("DownRPC", RpcTarget.All);
                Debug.Log("감속");
            }

            //스킬
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!curveWall)
                {
                    //v3 = (transform.position - new Vector3(0, 0, -1));
                    PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(SR.flipX ? 0.5f : -0.5f, -0.11f, 0), Quaternion.identity)
                        .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? 1 : -1);
                    //AN.SetTrigger("shot");
                }
                else {
                    PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(SR.flipX ? 0.5f : -0.5f, -0.11f, 0), Quaternion.identity)
                            .GetComponent<PhotonView>().RPC("DirRPC2", RpcTarget.All, SR.flipX ? 1 : -1, v3);
                    Debug.Log("v3");
                }
                Debug.Log("[curve]" + curveWall);
            }
        }
        //else if ((transform.position - curPos).sqrMagnitude >= 100) {
        else if ((transform.position - lerpPos).sqrMagnitude >= 100)
        {
            //transform.position = curPos;
            transform.position = lerpPos;
        }
        else
        {
            //transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime*10);
            transform.position = Vector3.Lerp(transform.position, lerpPos, Time.deltaTime * 10);
        }
    }

    void OnTriggerEnter2D(Collider collision)
    {
        if (collision.transform.tag == "Spling")
        {
            RB.AddForce(Vector3.up * spling_power, ForceMode2D.Impulse);
            Debug.Log("Spling");
        }
        else if (collision.transform.tag == "Drink")
        {
            //Speed = 15;
            //rigid.addForce(Vector3.right * Accele_power, ForceMode.Impulse);
            count_D = 0;
            InvokeRepeating("WakeUp_D", 1f, 10f);
        }
        else if (collision.transform.tag == "Banana")
        {
            if (SR.flipX == true)
                RB.AddForce(Vector3.left * banana_power, ForceMode2D.Impulse);
            else if (SR.flipX == false)
                RB.AddForce(Vector3.right * banana_power, ForceMode2D.Impulse);
        }
        else if (collision.transform.tag == "Accele")
        {
            count_A = 0;
            InvokeRepeating("WakeUp_A", 1f, 2f);
        }
        else if (collision.transform.tag == "Flag")
        {
            Debug.Log("Flag");
        }
        else if (collision.transform.tag == "Ground")
        {
            Debug.Log("Flag");
        }
        else if (collision.transform.tag == "CurveWall")
        {
            Debug.Log("CurveWall");
        }
    }
    void WakeUp_D()
    {
        Speed = 10.0f;
        Jump_Power = 500.0f;
        count_D += 1;
    }
    void WakeUp_A()
    {
        Speed = 7.0f;
        count_A += 1;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "ground")
        {
            isGround = true;    //Ground에 닿으면 isGround는 true
            jumpCount = 1;          //Ground에 닿으면 점프횟수가 1로 초기화됨
            curveWall = false;
        }
        
        if (col.gameObject.tag == "CurveWall")
        {
            isGround = true;
            jumpCount = 1;
            curveWall = true;    //Ground에 닿으면 isGround는 true
        }
    }

    /*private void OnCollisionExit2D(Collision2D col)
    {
        if (!(col.gameObject.tag == "CurveWall"))
        {
            curveWall = false;    //Ground에 닿으면 isGround는 true
        }
    }
    */

    [PunRPC]
    void FlipXRPC(float axis)
    {
        SR.flipX = axis == -1;
        //RB.AddForce(Vector2.right * axis);
    }
    //void FlipXRPC(float axis) => SR.flipX = axis == -1;

    [PunRPC]
    void JumpRPC()
    {
        //RB.velocity = Vector2.zero;
        //RB.AddForce(Vector2.up * 7, ForceMode.Impulse); //3d로 바꿔줘야함

        if (isGround)
        {
            jumpCount = 1;
            if (jumpCount == 1)
            {
                Jump();
                isGround = false;
                jumpCount = 0;
            }
        }
    }
    void Jump()
    {
        RB.AddForce(new Vector3(0, Jump_Power, 0));
    }

    [PunRPC]
    void DownRPC()
    {
        //RB.velocity = Vector2.zero;
        //RB.AddForce(Vector2.up * 7, ForceMode.Impulse); //3d로 바꿔줘야함

        if (isGround)
        {
            Down();
        }
    }

    void Down()
    {
        RB.velocity *= 0.0f;
    }

    public void Hit(Vector3 trans, int dir)
    {
        /*
         * MP.fillAmount -= 0.1f;
        if (MP.fillAmount <= 0)
        {
            //GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        */
        Debug.Log("[hit] hit trans");
        Debug.Log(trans);
        RB.AddForce(new Vector3(hitPower, 0, 0) * dir);
        //transform.Translate(new Vector3(5,5,5));

    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //변수동기화
        if (stream.IsWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(MP.fillAmount);
        }
        else {
            curPos = (Vector3)stream.ReceiveNext();
            MP.fillAmount = (float)stream.ReceiveNext();
        }
    }*/
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //변수동기화
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            //stream.SendNext(MP.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            //MP.fillAmount = (float)stream.ReceiveNext();

            /*
            if (prevPos == Vector3.zero)
            {
                prevPos = curPos;
            }

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp)); //속도

            lerpPos = prevPos + ((Vector3)RB.velocity * lag) + ((curPos - prevPos) / 2 * Time.deltaTime * Time.deltaTime);
            //lerpPos = prevPos + ((Vector3)RB.velocity * lag);

            prevPos = curPos;
            prevPos.z = -1;*/

            lerpPos = curPos;
        }
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "Run":
                audioSource.clip = audioRun;
                break;
            case "Fart":
                audioSource.clip = audioFart;
                break;
            case "Victory":
                audioSource.clip = audioVictory;
                break;
            case "Defeat":
                audioSource.clip = audioDefeat;
                break;
            case "Crush":
                audioSource.clip = audioCrush;
                break;
            case "Slip":
                audioSource.clip = audioSlip;
                break;
            case "Drink":
                audioSource.clip = audioDrink;
                break;
        }
        audioSource.Play();
    }
}
