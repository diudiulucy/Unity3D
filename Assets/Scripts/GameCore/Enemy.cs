using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Transform m_transform;
    Player m_player;//主角
    NavMeshAgent m_agent;//寻路组件
    //移动速度
    float m_movSpeed = 2.5f;
    //角色旋转速度
    float m_rotSpeeed = 30;
    //计时器
    float m_timer = 2;
    //生命值
    int m_life = 2;
    Animator m_ani;

    public float attackDistance = 1.5f;
    public int attack = 1;
    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        m_ani = this.GetComponent<Animator>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();//根据tag 找到主角
        //获取寻路组件
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.speed = m_movSpeed;//指定寻路器的行走速度

        Invoke("SetDestination", 2f);
        //设置寻路目标,如果希望自行控制移动，只能使用NavMeshAgent.CalculatePath 计算出路径，然后按路径节点移动
    }

    void RoatateTo()
    {
        Vector3 targetdir = m_player.m_transform.position - m_transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetdir, m_rotSpeeed * Time.deltaTime, 0.0f);//匀速转向target的位置
        m_transform.rotation = Quaternion.LookRotation(newDir);

    }

    void SetDestination()
    {
        m_agent.SetDestination(m_player.m_transform.position);
    }

    public void OnDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_ani.SetBool("death", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //如果主角生命为0，什么也不做
        if (m_player.m_life <= 0)
        {
            return;
        }

        //获取当前的动画状态
        AnimatorStateInfo stateInfo = m_ani.GetCurrentAnimatorStateInfo(0);

        //如果处于待机状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.idle") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("idle", false);
            //待机一段时间
            m_timer -= Time.deltaTime;
            if (m_timer > 0)
            {
                return;
            }

            if (Vector3.Distance(m_transform.position, m_player.transform.position) < attackDistance)
            {
                m_ani.SetBool("attack", true);
            }
            else
            {
                //重置计时器
                m_timer = 1;

                m_agent.SetDestination(m_player.m_transform.position);
                m_ani.SetBool("run", true);
            }
        }

        //如果处于跑步状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.run") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("run", false);
            //每隔一秒重新定位主角的位置
            m_timer -= Time.deltaTime;
            if (m_timer < 0)
            {
                m_agent.SetDestination(m_player.m_transform.position);
                m_timer = 1;
            }

            if (Vector3.Distance(m_transform.position, m_player.transform.position) < attackDistance)
            {
                m_agent.isStopped = true;//停止寻路
                m_ani.SetBool("attack", true);
            }
        }

        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.attack") && !m_ani.IsInTransition(0))
        {
            //面向主角
            RoatateTo();
            m_ani.SetBool("attack", false);
            //如果动画播完，重新进入待机状态
            if (stateInfo.normalizedTime >= 1.0f)
            {
                m_ani.SetBool("idle", true);

                //重置计时器
                m_timer = 2;

                m_player.OnDamage(attack);
            }
        }

        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.death") && !m_ani.IsInTransition(0))
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                //加分
                GameManager.Instance.SetScore(100);
                Destroy(this.gameObject);
            }
        }

    }
}
