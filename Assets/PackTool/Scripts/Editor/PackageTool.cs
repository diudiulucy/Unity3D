using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using TomatoJoy.Data;
using TomatoJoy.Tools;
using System.Linq;
using System.Reflection;

public class PackageTool : EditorWindow
{
    public static PackageTool instance;
    private const string localRootPath = "/PackTool";
    private const string configJsonPath = localRootPath + "/Data/channel.json";
    private static List<Channel> channelconfigs;
    private static int configShowOption = 0;
    private static int curChannelIndex = 0;

    private Vector2 channelListScrollPosition, channelInfoScrollPosition;
    private static string resourcePath, dataFilePath;
    private static Dictionary<string, string> updateEditCache = new Dictionary<string, string>();//编辑的缓冲

    private enum ConfigShowOption
    {
        info = 0,
        data = 1,
        setting = 2
    }
    private static Dictionary<string, string> configShowOptions = new Dictionary<string, string>{
        {ConfigShowOption.info.ToString(), "基础信息"},
        {ConfigShowOption.data.ToString(), "数据信息"},
        {ConfigShowOption.setting.ToString(),"配置信息"}
    };

    private static Dictionary<int, bool> isEditorDic = new Dictionary<int, bool>();

    [MenuItem("Build/打包工具")]
    public static void ShowWindow()
    {
        resourcePath = UnityEngine.Application.dataPath + localRootPath;
        instance = (PackageTool)EditorWindow.GetWindow(typeof(PackageTool), true, "打包工具");
        ReadConfigData();
        InitEditor();
    }


    static void InitEditor()
    {
        updateEditCache.Clear();
        isEditorDic.Clear();
        for (int i = 0; i < System.Enum.GetNames(typeof(ConfigShowOption)).Length; i++)
        {
            isEditorDic.Add(i, false);
        }
    }

    static void ClearEditor()
    {
        isEditorDic[0] = false;
        isEditorDic[1] = false;
        isEditorDic[2] = false;
    }

    public static void SetSplashScreen(string splashName)
    {
        var logo = new PlayerSettings.SplashScreenLogo();

        logo.duration = 2f; //设置闪屏时间
        PlayerSettings.SplashScreen.show = true;
        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SplashScreen.overlayOpacity = 0;
        PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.DarkOnLight;
        PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Static;
        PlayerSettings.SplashScreen.backgroundColor = Color.white;//设置闪屏背景颜色

        string sourcetexPath = string.Format("{0}/SplashLib/{1}.png", resourcePath, splashName);
        sourcetexPath = GetRelativeAssetPath(sourcetexPath);
        SetTextureFormat(sourcetexPath);
        //设置闪屏logo
        logo.logo = AssetDatabase.LoadAssetAtPath<Sprite>(sourcetexPath);
        PlayerSettings.SplashScreen.logos = new PlayerSettings.SplashScreenLogo[1] { logo };
        AssetDatabase.Refresh();
        Debug.Log("完成Splash替换!");
    }

    public static string getOutPutPath(string version)
    {
        var timeStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        var path = string.Format("../Output/{0}/Android/{1}/{2}_{3}.apk", Application.productName, version, timeStr, Application.productName);

        return path;
    }

    /// <summary>
    /// Building场景的 前处理
    /// OK 
    /// </summary>
    [PostProcessSceneAttribute(2)]
    public static void OnPostprocessScene()
    {


    }

    static void SetIcon(BuildTargetGroup group, string iconName)
    {
        string sourcetexPath = string.Format("{0}/IconLib/{1}.png", resourcePath, iconName);
        sourcetexPath = GetRelativeAssetPath(sourcetexPath);
        SetTextureFormat(sourcetexPath);
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(sourcetexPath, typeof(Texture2D)) as Texture2D;
        Texture2D[] icons = new Texture2D[] { sourcetex, sourcetex, sourcetex, sourcetex, sourcetex, sourcetex };
        PlayerSettings.SetIconsForTargetGroup(group, icons);
        AssetDatabase.Refresh();
        Debug.Log("完成Icon替换!");
    }

    static string GetRelativeAssetPath(string _fullPath)
    {
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    static void SetTextureFormat(string _relativeAssetPath)
    {
        TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
        if (ti == null)
        {
            Debug.LogError(string.Format("未找到 {0} 文件", _relativeAssetPath));
            return;
        }
        ti.textureType = TextureImporterType.Sprite;
        ti.isReadable = true;
        ti.mipmapEnabled = false;
        ti.maxTextureSize = 2048;
#if UNITY_5_6_OR_NEWER
        ti.textureCompression = TextureImporterCompression.Uncompressed;
#endif
        AssetDatabase.ImportAsset(_relativeAssetPath);
    }

    public static void PreBuildSetting()
    {
        var channelInfo = channelconfigs[curChannelIndex].channelInfo;
        var applicationIdentifier = channelInfo.BundleId;
        var productName = channelInfo.ProductName;
        var bundleVersion = channelInfo.BundleVersion;
        var bundleVersionCode = int.Parse(channelInfo.BundleVersionCode);
        var DefineSymbol = channelInfo.DefineSymbol;

        var buildStr = string.Format("包名:{0}\n 产品名:{1} \n 版本号:{2}\n 版本数{3} \n 宏定义{4} ", applicationIdentifier, productName, bundleVersion, bundleVersionCode, DefineSymbol);
        Debug.Log(buildStr);
        BuildTargetGroup buildGroup = BuildTargetGroup.Android;
        SetIcon(buildGroup, channelInfo.Icon);
        SetSplashScreen(channelInfo.Splash);
        SetAndroidSign();

        // 设置productName和版本号
        PlayerSettings.companyName = "QianXun";
        PlayerSettings.SetApplicationIdentifier(buildGroup, applicationIdentifier);
        PlayerSettings.productName = productName;
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        //打包相关设置
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
        PlayerSettings.Android.forceSDCardPermission = true;//允许读写SD卡


        PlayerSettings.SetScriptingBackend(buildGroup, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, DefineSymbol);//添加一个Debug的宏
        instance.ShowNotification(new GUIContent("已完成设置"));
    }

    // 设置签名
    static void SetAndroidSign()
    {
        PlayerSettings.Android.keystoreName = "../Keystore/minigame.keystore";
        PlayerSettings.Android.keystorePass = "minigame";
        PlayerSettings.Android.keyaliasName = "minigame";
        PlayerSettings.Android.keyaliasPass = "minigame";
    }

    public static void BuildAndroid(bool buildABB)
    {
        PreBuildSetting();
        //如果不是android平台，转为android平台
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(UnityEditor.BuildTargetGroup.Android, BuildTarget.Android);
        }
        // 设置Unity打包信息
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetBuildScenes();
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        EditorUserBuildSettings.buildAppBundle = buildABB;
        buildPlayerOptions.locationPathName = getOutPutPath(PlayerSettings.bundleVersion);
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static void ReadConfigData()
    {
        string TargetCodeFile = UnityEngine.Application.dataPath + configJsonPath;
        var text = File.ReadAllText(TargetCodeFile);

        channelconfigs = JsonUtility.FromJson<ChannelConfig>(text).channels;
    }

    void OnGUI()
    {
        if (channelconfigs != null && channelconfigs.Count <= 0)
        {
            return;
        }

        createSingleBuild();
    }

    void createSingleBuild()
    {
        GUILayout.Space(10);
        GUI.Box(new Rect(20, 20, 650, 310), "");
        createChannelSelectBox();
        createConfigShowOption();
        createChannelDetialInfo();
        createOperatorToolBar();
        createBuildOperator();
    }


    void createBuildOperator()
    {
        GUI.BeginGroup(new Rect(0, -50, Screen.width, 500));
        if (!isEditorDic[configShowOption])
        {
            EditorGUILayout.BeginHorizontal();
            if (GUI.Button(new Rect(250, 400, 100, 25), "仅设置"))
            {
                PreBuildSetting();
            }

            if (GUI.Button(new Rect(400, 400, 100, 25), "打包APK"))
            {
                BuildAndroid(false);
                GUIUtility.ExitGUI();
            }

            if (GUI.Button(new Rect(550, 400, 100, 25), "打包AAB"))
            {
                BuildAndroid(true);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {

        }
        GUI.EndGroup();
    }

    void createChannelSelectBox()
    {
        var groundWidth = 120;
        var groundHeight = 260;

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var groupx = 30;
        var groupy = 40;

        // GUILayout.Label("渠道选择");
        GUI.BeginGroup(new Rect(groupx, groupy, groundWidth, groundHeight));
        GUI.Box(new Rect(0, 0, groundWidth, groundHeight - 10), "");
        // GUILayout.Space(10);
        channelListScrollPosition = EditorGUILayout.BeginScrollView(channelListScrollPosition, GUILayout.Width(groundWidth), GUILayout.Height(225));
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < channelconfigs.Count(); i++)
        {
            var data = channelconfigs[i];
            if (GUILayout.Button(data.channelInfo.ChannelCN, GUILayout.Height(30)))
            {
                if (curChannelIndex != i)
                {
                    if (isEditorDic[configShowOption])
                    {//正在编辑
                        if (EditorUtility.DisplayDialog("提示", "\n您正在编辑\n" + "确定退出编辑吗？", "确定", "取消"))
                        {
                            isEditorDic[configShowOption] = false;
                            curChannelIndex = i;

                            AssetDatabase.Refresh();
                        }
                    }
                    else
                    {
                        curChannelIndex = i;
                    }
                }

            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUI.EndGroup();
    }

    void createConfigShowOption()
    {

        var groundWidth = 500;
        var groundHeight = 30;
        var groupx = 160;
        var groupy = 40;
        GUI.BeginGroup(new Rect(groupx, groupy, groundWidth, groundHeight));
        GUI.Box(new Rect(0, 0, groundWidth, groundHeight), "");
        int cOption = 0;
        cOption = GUI.Toolbar(new Rect(0, 0, groundWidth, groundHeight), configShowOption, configShowOptions.Values.ToArray());
        if (cOption != configShowOption)
        {
            configShowOption = cOption;
        }
        GUI.EndGroup();

    }

    void createChannelDetialInfo()
    {
        var groundWidth = 500;
        var groundHeight = 600;
        var groupx = 160;
        var groupy = -160;
        GUI.Box(new Rect(groupx, 70, groundWidth, 220), "");
        GUI.BeginGroup(new Rect(groupx, groupy, groundWidth, groundHeight));
        channelInfoScrollPosition = EditorGUILayout.BeginScrollView(channelInfoScrollPosition, GUILayout.Width(groundWidth), GUILayout.Height(200));
        EditorGUILayout.BeginVertical();
        object channelData = getChannelData();
        ShowChannelData(channelData);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        GUI.EndGroup();
    }

    object getChannelData()
    {
        var channel = channelconfigs[curChannelIndex];
        var channelInfo = channel.channelInfo;
        object channelData = channel.channelInfo;
        switch (configShowOption)
        {
            case (int)ConfigShowOption.info:
                channelData = channel.channelInfo;
                break;
            case (int)ConfigShowOption.data:
                channelData = channel.channelData;
                break;
            case (int)ConfigShowOption.setting:
                channelData = channel.channelMeta;
                break;
            default:
                break;
        }
        return channelData;
    }

    void createOperatorToolBar()
    {
        GUI.BeginGroup(new Rect(0, -100, Screen.width, 500));
        if (!isEditorDic[configShowOption])
        {
            EditorGUILayout.BeginHorizontal();

            if (GUI.Button(new Rect(450, 400, 80, 25), "创建渠道"))
            {
                EditorChannel.ShowWindow();
            }

            if (GUI.Button(new Rect(550, 400, 80, 25), "编辑"))
            {

                updateEditCache.Clear();
                object channelInfo = getChannelData();

                Type type = channelInfo.GetType();
                FieldInfo[] fields = type.GetFields();
                foreach (FieldInfo f in fields)
                {

                    var value = f.GetValue(channelInfo).ToString();
                    Debug.Log(value);
                    updateEditCache.Add(f.Name, value);
                }

                isEditorDic[configShowOption] = true;
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            if (GUI.Button(new Rect(450, 400, 80, 25), "更新"))
            {
                isEditorDic[configShowOption] = false;
                string updateContent = getUpdate();
                if (updateContent != null)
                {
                    if (EditorUtility.DisplayDialog("更新提示", "\n确认提交以下内容吗？\n" + updateContent, "确定", "取消"))
                    {
                        SaveEditorData();
                    }
                }
            }
            if (GUI.Button(new Rect(550, 400, 80, 25), "取消"))
            {
                isEditorDic[configShowOption] = false;
            }
            EditorGUILayout.EndHorizontal();
        }
        GUI.EndGroup();
    }

    void SaveEditorData()
    {

        object channelInfo = getChannelData();
        Type type = channelInfo.GetType();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo f in fields)
        {
            var value = f.GetValue(channelInfo).ToString();
            if (updateEditCache.ContainsKey(f.Name) && value != updateEditCache[f.Name])
            {
                type.GetField(f.Name).SetValue(channelInfo, updateEditCache[f.Name]);
            }
        }

        WriteConfigJson();
    }

    void WriteConfigJson()
    {
        string TargetCodeFile = UnityEngine.Application.dataPath + configJsonPath;
        if (!File.Exists(TargetCodeFile))
        {
            FileStream file = File.Create(TargetCodeFile);
            file.Close();
            AssetDatabase.Refresh();
        }
        ChannelConfig config = new ChannelConfig();
        config.channels = channelconfigs;
        string str = JsonUtility.ToJson(config);

        using (StreamWriter writer = new StreamWriter(TargetCodeFile, false))
        {
            try
            {
                ClearEditor();
                writer.WriteLine("{0}", str);
                instance.ShowNotification(new GUIContent("数据导出成功，路径：\n" + configJsonPath));
            }
            catch (System.Exception ex)
            {
                string msg = " 出错了:\n" + ex.ToString();
                Debug.LogError(msg);
            }
            finally
            {
                writer.Close();
                AssetDatabase.Refresh();
            }
        }
    }

    string getUpdate()
    {
        string result = null;
        object channelInfo = getChannelData();
        Type type = channelInfo.GetType();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo f in fields)
        {
            var value = f.GetValue(channelInfo).ToString();
            // Debug.Log("name= " + f.Name + " originValue= " + value + " updateValue=" + updateEditCache[f.Name]);
            if (updateEditCache.ContainsKey(f.Name) && updateEditCache[f.Name] != value)
            {
                result += BreakLongString("\n" + DataType.GetDataTypeName(f.Name) + ": " + updateEditCache[f.Name]);
            }
        }
        return result;
    }
    private const int breakLong = 50;
    public static string BreakLongString(string SubjectString)
    {
        if (SubjectString.Length < breakLong)
        {
            return SubjectString;
        }
        else
        {
            string tmp = null;
            for (int i = 0; i < SubjectString.Length; i++)
            {
                tmp += SubjectString[i];
                if (i % breakLong == 0 && i != 0)
                {
                    tmp += "\n";
                }
            }
            SubjectString = tmp;
        }
        return SubjectString;
    }

    static void ShowChannelData(object channelInfo)
    {
        // Debug.Log(JsonUtility.ToJson(channelInfo));
        Type type = channelInfo.GetType();
        // Debug.Log(channelInfo.GetType());
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo f in fields)
        {
            var value = f.GetValue(channelInfo).ToString();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(DataType.GetDataTypeName(f.Name), GUILayout.Width(150));
            if (!isEditorDic[configShowOption] || (f.Name == "ChannelCN" || f.Name == "AndroidLib"))
            {
                EditorGUILayout.SelectableLabel(value, GUILayout.Height(18));
            }
            else
            {
                Debug.Log(f.Name + ":" + value + ":" + updateEditCache[f.Name]);
                //这段没问题 不用改了，看怎么保存吧
                updateEditCache[f.Name] = EditorGUILayout.TextField(updateEditCache[f.Name], GUILayout.Height(18));
                if (value != updateEditCache[f.Name])
                {
                    Debug.Log("name = " + f.Name + " value= " + value + " EditValue = " + updateEditCache[f.Name]);
                    EditorGUILayout.LabelField("已更新", GUILayout.Width(40));
                }
                else
                {
                    EditorGUILayout.LabelField("", GUILayout.Width(40));
                }
            }
            EditorGUILayout.EndHorizontal();

        }
    }


    /// <summary>
    /// Building的 后处理
    /// todo 需要打开这个所在的路径（文件夹）   这个Unity会自动打开
    /// </summary>
    [PostProcessBuild(1)]
    public static void BuildFinished(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("Build Success  输出平台: " + target + "  输出路径: " + pathToBuiltProject);

        if (target == BuildTarget.Android)
        {

        }

#if UNITY_ANDROID
        instance.ShowNotification(new GUIContent("已完成打包"));
        string[] paths = pathToBuiltProject.Replace('\\', '/').Split('/');
        pathToBuiltProject = null;
        for (int i = 0; i < paths.Length - 1; i++)
        {
            pathToBuiltProject += paths[i] + "/";
        }
        Debug.Log("pathToBuiltProject = " + pathToBuiltProject);
        UnityEngine.Application.OpenURL(pathToBuiltProject);
#endif

    }

    //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }
}