using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField NickNameInlput;
    public GameObject DisconnectPanel;
    public GameObject WinPanel;
    public GameObject losePanel;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        //Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    //public void Connect() { PhotonNetwork.ConnectUsingSettings(); }

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        PhotonNetwork.LocalPlayer.NickName = NickNameInlput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        DisconnectPanel.SetActive(false);
        Debug.Log("패널제거");
        StartCoroutine("DestroyBullet");
        Spawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Bullet")) GO.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
    }

    public void Spawn()
    {
        //PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-0.08f, 9f), 0.39f, -1), Quaternion.identity);
        PhotonNetwork.Instantiate("Player", new Vector3(-0.08f, 0.39f, -1), Quaternion.identity);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);
        DisconnectPanel.SetActive(true);
    }
}