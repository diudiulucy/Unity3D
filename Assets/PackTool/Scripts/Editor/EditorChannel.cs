using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor.Callbacks;
using TomatoJoy.Data;

namespace TomatoJoy.Tools{
	/// 安卓编辑窗口
	public class EditorChannel : EditorWindow
	{
		private static EditorWindow instance;
		private static Dictionary<string, string>[] createChannelConfig;
		private static Dictionary<string, string>[] updateChannelConfig;
		private static int configShowOption;
		private static Vector2 channelInfoScrollPosition;
		private static string[] InfoOptions = new string[3]{ "基础信息", "数据信息", "配置信息"};
		private enum EditorType
		{
			UpdateChannel,
			CreateChannle
		}
		private static EditorType editorType;

		public static void ShowWindow ()
		{
			createChannelConfig = CreateChannelData ();
			editorType = EditorType.CreateChannle;
			if (instance != null) {
				instance.Close ();
			}
			instance = EditorWindow.GetWindow (typeof(EditorChannel), true, "创建渠道");
			InitEditor ();
		}
		public static void ShowWindow (Dictionary<string, string>[] channelConfig)
		{
			updateChannelConfig = channelConfig;
			editorType = EditorType.UpdateChannel;
			if (instance != null) {
				instance.Close ();
			}
			instance = EditorWindow.GetWindow (typeof(EditorChannel), true, "编辑渠道");
			InitEditor ();
		}

		static void InitEditor(){
			
		}

		void OnGUI ()
		{
			GUILayout.Space (10);
			var scrollViewH = 270;
			EditorGUILayout.LabelField (editorType == EditorType.CreateChannle ? "渠道信息" :"编辑渠道: " + updateChannelConfig[0]["ChannelCN"], GUILayout.Width (200));
			GUILayout.Space (5);
			configShowOption = GUILayout.Toolbar (configShowOption, InfoOptions);
			if (editorType == EditorType.UpdateChannel) {
				EditorGUILayout.BeginVertical ("box");
				channelInfoScrollPosition = EditorGUILayout.BeginScrollView (channelInfoScrollPosition, GUILayout.Height (scrollViewH));
				ShowUpdateChannelData (configShowOption);
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndScrollView ();
			}else if (editorType == EditorType.CreateChannle) {
//				EditorGUILayout.LabelField ("创建信息", GUILayout.Width (80));

				EditorGUILayout.BeginVertical ("box");
				channelInfoScrollPosition = EditorGUILayout.BeginScrollView (channelInfoScrollPosition, GUILayout.Height (scrollViewH));
				ShowUpdateChannelData (configShowOption);
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndScrollView ();
			}

			if (GUILayout.Button ("确认", GUILayout.Width (80), GUILayout.Height (25))) {
				AssetDatabase.SaveAssets ();
				AssetDatabase.Refresh ();
				instance.Close ();
				if (PackageTool.instance != null) {
					PackageTool.instance.ShowNotification (new GUIContent ("此功能开发中"));
				}
			}
		}

		void ShowUpdateChannelData (int configShowOption)
		{
			Dictionary<string, string> channelInfo = null;
			if (editorType == EditorType.CreateChannle) {
				channelInfo = createChannelConfig [configShowOption];
			} else if (editorType == EditorType.UpdateChannel) {
				channelInfo = updateChannelConfig [configShowOption];
			}
			foreach (string key in channelInfo.Keys) {
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (DataType.GetDataTypeName (key), GUILayout.Width (150));
				if (key == "ChannelCN" || key == "AndroidLib") {
					EditorGUILayout.SelectableLabel (channelInfo [key], GUILayout.Height (18));
				} else {
					EditorGUILayout.TextField (channelInfo[key], GUILayout.Height (18));
				}
				EditorGUILayout.EndHorizontal ();
			}
		}

		static Dictionary<string, string>[] CreateChannelData(){
			Dictionary<string, string>[] channelConfigTmp = new Dictionary<string, string>[3];
			channelConfigTmp [0] = new Dictionary<string, string> ();
			channelConfigTmp [1] = new Dictionary<string, string> ();
			channelConfigTmp [2] = new Dictionary<string, string> ();

			channelConfigTmp [0].Add ("ChannelId", "渠道ID(oppo)");
			channelConfigTmp [0].Add ("ChannelCN", "渠道名(OPPO)");
			channelConfigTmp [0].Add ("ProductName", "产品名(八分音符酱)");
			channelConfigTmp [0].Add ("BundleId", "包名(com.tomatojoy.bfyfj.nearme.gamecenter)");
			channelConfigTmp [0].Add ("BundleVersion", "版本号(1.0.0)");
			channelConfigTmp [0].Add ("ShortBundleVersion", "短版本号(1.0)");
			channelConfigTmp [0].Add ("BundleVersionCode", "版本数(1)");
			channelConfigTmp [0].Add ("Icon", "ICON图标(base)");
			channelConfigTmp [0].Add ("Splash", "闪屏(base)");
			channelConfigTmp [0].Add ("DefineSymbol", "宏定义(oppo;gdt)");
			channelConfigTmp [0].Add ("AndroidLib", "SDK Android Lib(Android_oppo)");

			channelConfigTmp [1].Add ("TjAppKey", "20171201094256110-5803");
			channelConfigTmp [1].Add ("UMENG_APPKEY", "598290094544cb17150008bf");
			channelConfigTmp [1].Add ("TalkingDataKey", "6A1FFFC931C74191A5CFDE989001A8C8");
			channelConfigTmp [1].Add ("ReyunKey", "5302c3b65817d8aff1ef1d0d8d63939e");
			channelConfigTmp [1].Add ("GdtAppId", "1106326490");
			channelConfigTmp [1].Add ("GdtBannerKey", "2010220774028768");
			channelConfigTmp [1].Add ("GdtInsertKey", "7000725704820739");
			channelConfigTmp [1].Add ("GdtSplashKey", "1010926774820830");
			channelConfigTmp [1].Add ("OppoAppId", "3607486");
			channelConfigTmp [1].Add ("OppoBannerKey", "7280");
			channelConfigTmp [1].Add ("OppoInsertKey", "7281");
			channelConfigTmp [1].Add ("OppoSplashKey", "7282");

			channelConfigTmp [2].Add ("TjAppKey", "20171201094256110-5803");
			channelConfigTmp [2].Add ("GdtAppId", "1106326490");
			channelConfigTmp [2].Add ("GdtSplashKey", "1010926774820830");
			channelConfigTmp [2].Add ("OppoAppId", "3607486");
			channelConfigTmp [2].Add ("OppoSplashKey", "7282");

			return channelConfigTmp;
		}
	}
}