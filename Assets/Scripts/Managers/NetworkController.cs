using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    #region Properties
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
        GameManager.Instance.GameUIController.HideConnectingPanel();
    }
    #endregion

    #region Private Methods

    #endregion
}
