using UnityEditor;
using UnityEngine;
using System.IO;
public class ExportAssetBundles : EditorWindow
{
    //[MenuItem("Tool/Create AssetBundle All")]
    //private static void CreateAssetBundle()
    //{
    //    Debug.Log("Create AssetBundle...");
    //    BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/assetbundle", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    //    AssetDatabase.Refresh();
    //}

    [MenuItem("Tool/Build One Selected Obj Bundle/Android")]
    //设为静态方法
    static void Build1SelectedBundleAN()
    {
         Debug.Log("=====================开始");
        //备打文件集合，当前为单个
        AssetBundleBuild[] abb = new AssetBundleBuild[1];
        //文件路径（当前为单个，多个文件使用数组的形式）
        string[] path = new string[1];
      
        //获取文件路径（得到鼠标选中的文件）；
        path[0] = AssetDatabase.GetAssetPath(Selection.activeObject);

        Debug.Log(path[0]);
        //备打文件路径名，从Asset/开始
        abb[0].assetNames = path;
        //包名
        abb[0].assetBundleName = "test";
        //打包（“输出路径”，备打文件集合，打包设置，目标平台）
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, abb, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        AssetDatabase.Refresh();
    }


    [MenuItem("Tool/Create AssetBunldes")]
    [System.Obsolete]
    private static void CreateAssetBunldesMain()
    {
        //获取在Project视图中选择的所有游戏对象  
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //遍历所有的游戏对象  
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            string filename = GetFileName(sourcePath);
            string parentname = GetParentName(sourcePath);

            //Debug.Log(obj.name + " : " + sourcePath);

            if (!string.IsNullOrEmpty(filename))
            {
                Debug.Log("打包 " + obj.name + " : " + sourcePath);

                AssetImporter ai = AssetImporter.GetAtPath(sourcePath);
                if (ai.assetBundleName != parentname + ".assetbundle")
                {
                    ai.assetBundleName = parentname + ".assetbundle";
                }
            }
        }

        string path = Application.streamingAssetsPath + "/assetbundle";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath + "/assetbundle", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        //刷新编辑器  
        AssetDatabase.Refresh();
    }
    private static string GetParentName(string path)
    {
        string[] strs = path.Split('/');
        //string parent = strs[strs.Length - 2];
        for (int i = 0; i < strs.Length; i++)
        {
            if (strs[i].Contains("_ab")) return strs[i];
        }
        return "";
    }

    private static string GetFileName(string path)
    {
        string[] strs = path.Split('/');
        string filename = strs[strs.Length - 1];

        if (filename.Contains("."))
        {
            return filename;
        }
        else
        {
            return "";

        }
    }
}