using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Transform m_enemy;
    //生成的敌人的数量
    public int m_enemyCount = 0;

    //敌人最大生成数量
    public int m_maxEnemy = 3;

    //生成敌人的时间间隔
    public float m_timer = 3;

    protected Transform m_transform;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_enemyCount >= m_maxEnemy)
        {
            return;
        }

        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            //得到下一轮生成敌人的时间间隔，最大15秒，最小5秒
            m_timer = Random.Range(5,15);
            //生成敌人
            Transform obj = (Transform)Instantiate(m_enemy,m_transform.position,Quaternion.identity);
            //获取敌人脚本
            Enemy enemy = obj.GetComponent<Enemy>();
            //初始化敌人
            enemy.Init(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position,"item.png",true);
    }
}
