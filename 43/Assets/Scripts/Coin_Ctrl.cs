using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Ctrl : MonoBehaviour
{
    public float attractRange = 5f; // 코인이 플레이어에게 끌려가는 범위
    public float attractSpeed = 5f; // 코인이 플레이어에게 끌려가는 속도
    private Transform player;



    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < attractRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * attractSpeed * Time.deltaTime;
        }

        else
        {
            attractSpeed = 0;
        }

    }

    

   


}
