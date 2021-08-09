
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidToUnity : MonoBehaviour
{

    public Text text; //用来显示结果值
    private AndroidJavaClass androidJavaClass; //获取Unity发布的唯一MainActivity类
    private AndroidJavaObject androidJavaObject;//获取Activity对象

    public void Awake()
    {
        text = this.GetComponent<Text>();
        //Unity要导出的MainActivity类
        androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //获取MainActivity的实例对象
        androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void Start()
    {
        Add();
    }

    public void Add()
    {
        //调用Android写好的add的方法，并传参，获取结果值
        int sum = androidJavaObject.Call<int>("Add", 5, 5);
        text.text = "从Android计算出来的值为:" + sum;
    }
}