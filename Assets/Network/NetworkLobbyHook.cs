using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        SurvivorNetworkSetUp localPlayer = gamePlayer.GetComponent<SurvivorNetworkSetUp>();

        localPlayer.ID = lobbyPlayer.GetComponent<NetworkIdentity>().connectionToClient.connectionId;

        localPlayer.playerName = lobby.playerName;
        localPlayer.playerColor = lobby.playerColor;
    }
}
