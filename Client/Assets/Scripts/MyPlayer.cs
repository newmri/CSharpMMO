using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    void Start()
    {
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        StartCoroutine("CoSendPacket");
    }

    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            CGS_Move move = new CGS_Move();
            move.posX = UnityEngine.Random.Range(-50.0f, 50.0f);
            move.posY = 0.0f;
            move.posZ = UnityEngine.Random.Range(-50.0f, 50.0f);

            _network.Send(move.Write());
        }
    }

    NetworkManager _network;
}
