using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class TPServer : NetworkManager
    {
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log($"[SERVER] - new client has connected");
        }
    }
}