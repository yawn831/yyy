using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public enum EnemyState
{
    NomalState,
    FightingState,
    MovingState,
    RestingState
}

public class Enemy : MonoBehaviour
{
    private EnemyState state = EnemyState.NomalState;
    private EnemyState childState = EnemyState.RestingState;
    private NavMeshAgent enemyAgent;

    public float HP = 100;
    public float resetTime = 2;
    private float resetTimer = 0;

    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(state == EnemyState.NomalState)
        {
            if(childState == EnemyState.RestingState)
            {
                resetTimer += Time.deltaTime;
                
                if(resetTimer > resetTime)
                {
                    Vector3 randomPosition = FindRandomPosition();
                    enemyAgent.SetDestination(randomPosition);
                    childState = EnemyState.MovingState;
                }
            }
            else if(childState == EnemyState.MovingState)
            {
                if(enemyAgent.remainingDistance <= 0)
                {
                    resetTimer = 0;
                    childState = EnemyState.RestingState;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(30);
        }
    }


    Vector3 FindRandomPosition()
    {
        // 获得一个随机方向
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1, 1f), 0, UnityEngine.Random.Range(-1, 1f));
        // 得到新的位置
        return transform.position + randomDir.normalized * UnityEngine.Random.Range(2, 5);
    }

    public void TakeDamage(float damage)
    {
        HP = MathF.Max(0, HP - damage);

        if(HP <= 0)
        {
            GetComponent<Collider>().enabled = false;
            int count = 4;
            for(int i = 0; i < count; i++)
            {
                SpwanPickableItem();
            }

            Destroy(this.gameObject);
        }
    }

    private void SpwanPickableItem()
    {
        ItemSO item = ItemDBManager.Instance.GetRandomItem();

        GameObject go = GameObject.Instantiate(item.prefab, transform.position, Quaternion.identity);
        go.tag = Tag.INTERACTABLE; // 设置tag

        PickableObject po = go.AddComponent<PickableObject>();
        po.itemSO = item;

        Collider collider = go.GetComponent<Collider>();
        if(collider != null)
        {
            collider.enabled = true;
            collider.isTrigger = false;
        }

        Rigidbody rgd = go.GetComponent<Rigidbody>();
        if(rgd != null)
        {
            rgd.isKinematic = false;
            rgd.useGravity = true;
        }

        Animator anim = go.GetComponent<Animator>(); // 停止动画
        if (anim != null)
        {
            anim.enabled = false;
        }
    }
}
