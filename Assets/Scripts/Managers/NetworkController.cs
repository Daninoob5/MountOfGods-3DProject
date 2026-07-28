using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NetworkController : MonoBehaviourPunCallbacks
{
    #region Properties
    public event Action JoinedRoom;
    #endregion

    #region Fields
    #endregion

    #region Unity Callbacks
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion

    #region Public Methods
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("MountOfGodsRoom", new RoomOptions {MaxPlayers = 10}, null);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Player",Vector3.zero,Quaternion.identity);
        GameManager.Instance.GameUIController.HideConnectingPanel();
        JoinedRoom?.Invoke();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (PhotonNetwork.IsMasterClient)
        {

        }
        base.OnDisconnected(cause);
    }
    public void GenerateTerrain(Vector2Int position, int prefabIndex)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_GenerateTerrain), RpcTarget.All, position.x, position.y, prefabIndex);
        }
    }
    public void NewDay(int dayNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_NewDay), RpcTarget.All, dayNumber);
        }
    }
    public void UpdatePoints(float points)
    {
        photonView.RPC(nameof(RPC_UpdatePoints), RpcTarget.All, points);
    }
    public void RemoteContinuePlaying()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_RemoteContinuePlaying), RpcTarget.Others);
    }
    public void CheckPlayersAlive()
    {
        photonView.RPC(nameof(RPC_CheckPlayersAlive), RpcTarget.MasterClient);
    }
    public void AddRemotePlayer(int viewID)
    {
        photonView.RPC(nameof(RPC_AddRemotePlayer), RpcTarget.Others, viewID);
    }
    public void IsAliveUpdate(bool alive, int viewID)
    {
        photonView.RPC(nameof(RPC_IsAliveUpdate), RpcTarget.All, alive, viewID);
    }
    public void ExitGameAll()
    {
        photonView.RPC(nameof(RPC_ExitGameAll), RpcTarget.All);
    }
    #endregion

    #region Private Methods
    [PunRPC]
    private void RPC_GenerateTerrain(int x, int y, int prefab)
    {
        GameManager.Instance.TerrainManager.GenerateNewPlot(new Vector2Int(x, y), prefab);
    }

    [PunRPC]
    private void RPC_NewDay(int dayNumber)
    {
        GameManager.Instance.GameTimeManager.RaiseNewDay(dayNumber);
    }

    [PunRPC]
    private void RPC_UpdatePoints(float points)
    {
        GameManager.Instance.ActualGodPoints = points;
        GameManager.Instance.UpdateGoalPercentage();
    }

    [PunRPC]
    private void RPC_RemoteContinuePlaying()
    {
        GameManager.Instance.GameUIController.DeathText.SetActive(false);
        GameManager.Instance.GameUIController.ShowInGameMenu();
    }

    [PunRPC]
    private void RPC_AddRemotePlayer(int viewID)
    {
        GameManager.Instance.RemotePlayers.Add(PhotonView.Find(viewID).GetComponent<Player>());
    }

    [PunRPC]
    public void RPC_CheckPlayersAlive()
    {
        GameManager.Instance.CheckPlayersAlive();
    }

    [PunRPC]
    public void RPC_IsAliveUpdate(bool alive, int viewID)
    {
        PhotonView.Find(viewID).GetComponent<Player>().Alive = alive;
    }

    [PunRPC]
    public void RPC_ExitGameAll()
    {
        GameManager.Instance.ExitGame();
    }
    #endregion
}
