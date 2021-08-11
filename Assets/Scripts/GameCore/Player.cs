using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform m_transform;
    CharacterController m_ch;
    public LayerMask m_layer;//射击时，射线能碰到的碰撞层 设为enemy 和level 这样主角射击时，其射线可以集中敌人和场景
    public Transform m_fx;//射中后的粒子效果
    public AudioClip m_audio;
    Transform m_camTransform;
    Vector3 m_camRot;
    Transform m_muzzlepoint;//枪口transform
    float m_camHeight = 1.4f;
    public int m_life = 5;
    float m_gravity = 2.0f;
    float m_movSpeed = 3.0f;
    float m_shootTimer = 0;
    public float attackDis = 100; 
    public int attack = 1;
    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        m_ch = this.GetComponent<CharacterController>();
        m_camTransform = Camera.main.transform;

        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;

        m_camTransform.rotation = m_transform.rotation;
        m_camRot = m_camTransform.eulerAngles;

        m_muzzlepoint = m_camTransform.Find("M16/weapon/muzzlepoint").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_life <= 0)
            return;
        Control();
    }

    void Control()
    {
        float rh = Input.GetAxis("Mouse X");
        float rv = Input.GetAxis("Mouse Y");
        m_camRot.x -= rv;
        m_camRot.y += rh;
        m_camTransform.eulerAngles = m_camRot;

        Vector3 camrot = m_camTransform.eulerAngles;
        camrot.x = 0; camrot.z = 0;

        m_transform.eulerAngles = camrot;

        float xm = 0, ym = 0, zm = 0;
        // ym -= m_gravity * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
        {
            zm += m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            zm -= m_movSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            xm -= m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            xm += m_movSpeed * Time.deltaTime;
        }

        m_ch.Move(m_transform.TransformDirection(new Vector3(xm, ym, zm)));

        Vector3 pos = m_transform.position;
        pos.y += m_camHeight;
        m_camTransform.position = pos;
        m_shootTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && m_shootTimer <= 0)
        {
            m_shootTimer = 0.1f;

            this.GetComponent<AudioSource>().PlayOneShot(m_audio);
            GameManager.Instance.SetAmmo(1);//减少弹药
            //用来保存射线的探测结果
            RaycastHit info;
            //从m_muzzlepoint的位置，向摄像机面向的正方向射出一根射线
            bool hit = Physics.Raycast(m_muzzlepoint.position, m_camTransform.TransformDirection(Vector3.forward), out info, attackDis, m_layer);
            if (hit)
            {
                if (info.transform.tag.CompareTo("enemy") == 0)
                {
                    Enemy enemy = info.transform.GetComponent<Enemy>();
                    // Debug.Log("击中敌人");
                    enemy.OnDamage(attack);
                }

                //在射中的地方设置一个粒子效果
                Instantiate(m_fx, info.point, info.transform.rotation);
            }

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
    }

    public void OnDamage(int damage)
    {
        m_life -= damage;
        //更新UI
        GameManager.Instance.SetLife(m_life);
        //如果生命0，取消鼠标锁定
        if (m_life <= 0)
        {
            // Screen.lockCursor = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }
}
