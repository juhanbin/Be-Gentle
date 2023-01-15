using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletScript : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    int dir;
    Vector3 v3;
    public SpriteRenderer SR;

    void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    void Update()
    {
        if (v3 == Vector3.zero)
        {
            transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);
            //Debug.Log("if");
        }
        else
        {
            transform.Translate(v3.normalized * 7 * Time.deltaTime * dir);
            //Debug.Log("else");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[hit]enterTrigger");
        if (collision.tag == "Ground")
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);

        if (!PV.IsMine && collision.tag == "Player" && collision.GetComponent<PhotonView>().IsMine)
        {
            collision.GetComponent<PlayerScript>().Hit(v3.normalized, dir);
            Debug.Log("[hit]hit ½ÇÇà");
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DirRPC(int dir)
    {
        this.dir = dir;
        this.v3 = Vector3.zero;
        SR.flipX = dir == 1;
    }

    [PunRPC]
    void DirRPC2(int dir, Vector3 v3)
    {
        this.dir = dir;
        this.v3 = v3;
        SR.flipX = dir == 1;
        Debug.Log("rpc2");
        Debug.Log(v3);
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }
}
