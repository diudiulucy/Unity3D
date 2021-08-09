using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{
     public float updateInterval = 0.5F;
     private float accum = 0; 
     private int frames = 0;

    private float timeleft;

    void Start()
     {
         timeleft = updateInterval;
#if UNITY_ANDROID&& !UNITY_EDITOR
         _getJavaObject();
         Invoke("callAndroid", 2);
#endif
        DontDestroyOnLoad(gameObject);
     }

    void Update()
     {
         _updateFPS();
     }

    #region FPS显示

    private float fps;
     [Range(0, 150)]
     public int MaxFPS;
     private string _curFPS;
     private void _updateFPS()
     {
         timeleft -= Time.deltaTime;
         //timescale是时间速度，为1时即为正常速度，为2时即为两倍速，为0时即为暂停
         accum += Time.timeScale / Time.deltaTime;
         ++frames;

        if (timeleft <= 0.0)
         {
             fps = accum / frames;
             _curFPS = "FPS:" + fps;
             timeleft = updateInterval;
             accum = 0.0F;
             frames = 0;
         }
     }

    #endregion

    #region CallAndroid

    private string _memoryInfo;
     private AndroidJavaObject ajo;
#if UNITY_ANDROID
     private void callAndroid()
     {
         _memoryInfo = string.Empty;
         string result = ajo.Call<string>("getMemoryInfo");
         string[] strArray = result.Split('-');
         //系统总内存
         _memoryInfo += "系统内存:" + strArray[0] + "\n";
         //系统可用内存
         _memoryInfo += "可用内存:" + strArray[1] + "\n";
         //App占用内存
         _memoryInfo += "app占用内存:" + strArray[2] + "\n";
         ////系统分配的最大占用内存
         //_memoryInfo += "AllotMemory:" + strArray[3];

        //加入代码之后，可能会每隔两秒钟出现卡顿现象
         Invoke("callAndroid", 2);
     }

    private void _getJavaObject()
     {
         AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
         ajo = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
     }
#endif
     #endregion

    void OnGUI()
     {
         GUIStyle style = new GUIStyle();
         style.fontSize = 20;
         style.normal.textColor = Color.green;
         GUI.Label(new Rect(Screen.width - 300, 10, 250, 200), _memoryInfo, style);

        style.normal.textColor = fps > MaxFPS ? Color.red : Color.green;
         GUI.Label(new Rect(Screen.width - 200, Screen.height - 50, 100, 30), _curFPS, style);
     }
}