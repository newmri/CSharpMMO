using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public void Add(GSC_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach(GSC_PlayerList.Player player in packet.players)
        {
            GameObject gameObject = Object.Instantiate(obj) as GameObject;

            if (player.isSelf)
            {
                MyPlayer myPlayer = gameObject.AddComponent<MyPlayer>();
                myPlayer.PlayerID = player.playerID;
                myPlayer.transform.position = new Vector3(player.posX, player.posY, player.posZ);
                _myplayer = myPlayer;
            }
            else
            {
                Player otherPlayer = gameObject.AddComponent<Player>();
                otherPlayer.PlayerID = player.playerID;
                otherPlayer.transform.position = new Vector3(player.posX, player.posY, player.posZ);
                _players.Add(player.playerID, otherPlayer);
            }
        }
    }

    public void EnterGame(GSC_BroadcastEnterGame packet)
    {
        if (packet.playerID == _myplayer.PlayerID)
            return;

        Object obj = Resources.Load("Player");
        GameObject gameObject = Object.Instantiate(obj) as GameObject;

        Player otherPlayer = gameObject.AddComponent<Player>();
        otherPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        _players.Add(otherPlayer.PlayerID, otherPlayer);
    }

    public void LeaveGame(GSC_BroadcastLeaveGame packet)
    {
        if (_myplayer.PlayerID == packet.playerID)
        {
            GameObject.Destroy(_myplayer.gameObject);
            _myplayer = null;
        }
        else
        {
            Player otherPlayer = null;
            if (_players.TryGetValue(packet.playerID, out otherPlayer))
            {
                GameObject.Destroy(otherPlayer.gameObject);
                _players.Remove(otherPlayer.PlayerID);
            }
        }
    }

    public void Move(GSC_BroadcastMove packet)
    {
        if (_myplayer.PlayerID == packet.playerID)
        {
            _myplayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
        }
        else
        {
            Player otherPlayer = null;
            if (_players.TryGetValue(packet.playerID, out otherPlayer))
            {
                otherPlayer.transform.position = new Vector3(packet.posX, packet.posY, packet.posZ);
            }
        }
    }

    public static PlayerManager Instance { get; } = new PlayerManager();

    MyPlayer _myplayer;
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

}
