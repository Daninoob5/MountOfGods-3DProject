using Photon.Pun;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (photonView.IsMine)
        {
            GameManager.Instance.LocalPlayer = gameObject.GetComponent<Player>();
            GameManager.Instance.SetLocalPlayer();
        }
        else
        {
            Player remotePlayer = gameObject.GetComponent<Player>();
            GameManager.Instance.RemotePlayers.Add(remotePlayer);
            remotePlayer.Local = false;
            transform.parent.gameObject.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.SetActive(false);
            gameObject.GetComponent<StarterAssetsInputs>().enabled = false;
            gameObject.GetComponent<CharacterController>().enabled = false;
            gameObject.GetComponent<FirstPersonController>().enabled = false;
            gameObject.GetComponent<PlayerInput>().enabled = false;
        }
}
}
