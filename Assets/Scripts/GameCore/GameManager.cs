using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    Player m_player;
    public int m_score = 0;
    public static int m_hiscore = 0;
    public int m_ammo = 100;

    public Text txt_ammo;
    public Text txt_hiscore;
    public Text txt_life;
    public Text txt_score;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit(); 
    }

    //更新分数
    public void SetScore(int score)
    {
        m_score += score;

        if (m_score > m_hiscore)
            m_hiscore = m_score;

        txt_score.text = "Score <color=yellow>" + m_score + "</color>"; ;
        txt_hiscore.text = "High Score " + m_hiscore;

    }

    //更新弹药
    public void SetAmmo(int ammo)
    {
        m_ammo -= ammo;
        if (m_ammo <= 0)
            m_ammo = 100 - m_ammo;

        txt_ammo.text = m_ammo.ToString() + "/100";
    }

    //更新生命值
    public void SetLife(int life)
    {
        txt_life.text = life.ToString();
    }
}
