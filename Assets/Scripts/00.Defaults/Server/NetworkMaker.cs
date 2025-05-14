using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Transporting;
using FishNet.Managing;
using UnityEngine;

public class NetworkMaker : MonoBehaviour
{
    private NetworkManager _networkManager;
    private DataManager _dataManager;
    private LocalConnectionState _clientState = LocalConnectionState.Stopped;
    private LocalConnectionState _serverState = LocalConnectionState.Stopped;
    void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        _dataManager = FindObjectOfType<DataManager>();
        if (_networkManager == null)
        {
            return;
        }
        else
        {
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }
    }
    private void OnDestroy()
    {
        if (_networkManager == null)
            return;

        _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
        _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
    }
    
    private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        _clientState = obj.ConnectionState;
    }


    private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        _serverState = obj.ConnectionState;
    }
    
    public void OnClick_Server()
    {
        if (_networkManager == null)
            return;

        if (_serverState != LocalConnectionState.Stopped)
            _networkManager.ServerManager.StopConnection(true);
        else
            _networkManager.ServerManager.StartConnection();
    }


    public void OnClick_Client()
    {
        if (_networkManager == null)
            return;
        
        if (_clientState != LocalConnectionState.Stopped)
        {
            _networkManager.ClientManager.StopConnection();
        }
        else
        {
            _networkManager.ClientManager.StartConnection();
        }
    }
}
