using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
        int nextServerToTry = 0;

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        //  if(connectedAtLeastOnce)
         if(MainNetworkPlayer.Main != null) {
            // assume kicked out of server
            if(!PlaygroundMaster.Instance.IsShowingBlock())
                PlaygroundMaster.Instance.ShowBlockCanvas("Client is disconnected from server\nPlease try again");
        }

        // // how to know if this is by server rejection (need to block)
        // // or is it because server not available (full and need to redirect)
        Debug.Log("OnClientDisconnect called on attempt " + (nextServerToTry-1));
        // Debug.Log("Was connected " + connectedAtLeastOnce.ToString());
        // if(connectedAtLeastOnce) {
        //     // assume kicked out of server
        //     if(!PlaygroundMaster.Instance.IsShowingBlock())
        //         PlaygroundMaster.Instance.ShowBlockCanvas("Client is disconnected from server\nPlease try again");
        // } else {
        //     // assume server is not available
        //     nextServerToTry++;
        //     if(nextServerToTry >= PlayFabMaster.Instance.serverDetails.serverCount()) {
        //         // base.OnClientDisconnect(conn);
        //         PlaygroundMaster.Instance.ShowBlockCanvas("Failed to connect to any of available servers\nServers are likely to be full\nPlease try again");
        //     } else {
        //         // Debug.Log("Retrying");
        //         // base.OnClientDisconnect(conn);
        //         // StartClientConnectionToOneOfListedServers();
        //     }
        //     //retry;
        // }


        base.OnClientDisconnect(conn);
        // Invoke(nameof(StopClient), 0.2f);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        // Debug.Log("OnClientConnec reade " + conn.isReady);
        PlaygroundMaster.Instance.SetServerTextNumber(nextServerToTry-1+1);

        // if truly success connection
        // if connected but will be rejected
    }
    public void StartClientConnectionToOneOfListedServers() {
        startConnecting = true;
    }

    bool startConnecting = false;
    bool stopTrying = false;
    private void Update() {
        if(!stopTrying && startConnecting && !MainNetworkPlayer.createdAsMainPlayer) {
            if(!NetworkClient.active) {
                // try connecting
                if(nextServerToTry >= PlayFabMaster.Instance.serverDetails.serverCount()) {
                     PlaygroundMaster.Instance.ShowBlockCanvas("Failed to connect to any of available servers.\nAll Servers are likely to be full. Please try again later.");
                     stopTrying = true;
                } else {
                    this.networkAddress = PlayFabMaster.Instance.serverDetails.getServerAddress(nextServerToTry);
                    this.GetComponent<Mirror.SimpleWeb.SimpleWebTransport>().port = PlayFabMaster.Instance.serverDetails.getServerPort(nextServerToTry);
                    nextServerToTry++;
                    PlaygroundMaster.Instance.ShowBlockCanvas("Connecting to server #" + nextServerToTry);
                    StartClient();
                }
            } else {
                // perhaps connecting
                // do nothing
            }
        }
    }
}
