﻿using UnityEngine;
// using UnityEngine.Collections;
// using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TomatoJoy.Data{

	public class DataType{

		static Dictionary<string,string> typeNameDic = new Dictionary<string, string> ();

		private static void InitTypeName(){
			typeNameDic.Clear ();

			#region 基础信息中文名

			typeNameDic.Add (	"ChannelId", 				"渠道标记"			);
			typeNameDic.Add (	"ChannelCN", 				"渠道名"				);
			typeNameDic.Add (	"Platform", 				"平台"				);
			typeNameDic.Add (	"ProductName", 				"产品名"				);
			typeNameDic.Add (	"BundleId", 				"包名"				);
			typeNameDic.Add (	"BundleVersion", 			"版本号"				);
			typeNameDic.Add (	"ShortBundleVersion", 		"短版本号"			);
			typeNameDic.Add (	"BundleVersionCode", 		"版本数"				);
			typeNameDic.Add (	"Icon", 					"Icon图标"			);
			typeNameDic.Add (	"Splash", 					"闪屏图片"			);
			typeNameDic.Add (	"DefineSymbol", 			"宏定义"				);
			typeNameDic.Add (	"AndroidLib", 				"SDK路径"			);

			#endregion

			#region 数据信息中文名

			typeNameDic.Add (	"UMENG_APPKEY", 			"友盟统计Key"			);
			typeNameDic.Add (	"TalkingDataKey", 			"TD统计Key"			);
			typeNameDic.Add (	"ApplovinKey", 				"Applovin广告Key"	);
			typeNameDic.Add (	"VungleKey", 				"Vungle广告Key"		);
			typeNameDic.Add (	"WeixinKey", 				"微信分享Key"			);
			typeNameDic.Add (	"WeixinSecret", 			"微信分享Secret"		);
			typeNameDic.Add (	"QqKey", 					"QQ分享Key"			);
			typeNameDic.Add (	"QqSecret", 				"QQ分享Secret"		);
			typeNameDic.Add (	"SinaKey", 					"新浪分享Key"			);
			typeNameDic.Add (	"SinaSecret", 				"微博分享Secret"		);
			typeNameDic.Add (	"SharePath", 				"分享地址"			);
			typeNameDic.Add (	"GdtAppId", 				"广点通广告ID"		);
			typeNameDic.Add (	"GdtBannerKey", 			"广点通BannerKey"		);
			typeNameDic.Add (	"GdtInsertKey", 			"广点通插屏Key"		);
			typeNameDic.Add (	"GdtSplashKey", 			"广点通开屏Key"		);
			typeNameDic.Add (	"GdtNativeKey", 			"广点通原生Key"		);
			typeNameDic.Add (	"AppleId", 					"苹果ID"				);

			typeNameDic.Add (	"MeizuAppId", 				"魅族ID"				);
			typeNameDic.Add (	"MeizuAppKey", 				"魅族Key"			);
			typeNameDic.Add (	"YumiId", 					"玉米广告Key"			);
			typeNameDic.Add (	"M4399Key", 				"4399支付Key"		);
			typeNameDic.Add (	"AppName", 					"游戏名"				);
			typeNameDic.Add (	"TjAppKey", 				"屏蔽地区Key"			);
			typeNameDic.Add (	"AwakeTime", 				"唤醒广告时间"			);
			typeNameDic.Add (	"OppoAppId", 				"Oppo广告ID"			);
			typeNameDic.Add (	"OppoBannerKey", 			"Oppo横幅ID"			);
			typeNameDic.Add (	"OppoInsertKey", 			"Oppo插屏ID"			);
			typeNameDic.Add (	"OppoAppKey", 				"Oppo支付Key"		);
			typeNameDic.Add (	"OppoAppSecret", 			"Oppo支付Secret"		);
			typeNameDic.Add (	"LenovoBannerKey", 			"联想横幅Key"			);
			typeNameDic.Add (	"LenovoInsertKey", 			"联想插屏Key"			);
			typeNameDic.Add (	"LenovoIncentVideoKey", 	"联想激励视频Key"		);
			typeNameDic.Add (	"VivoAppId", 				"Vivo应用ID"			);
			typeNameDic.Add (	"VivoCpId", 				"Vivo商户ID"			);
			typeNameDic.Add (	"VivoAppKey", 				"Vivo秘钥Key"		);
			typeNameDic.Add (	"IapPayAppId", 				"爱贝支付应用编号"		);
			typeNameDic.Add (	"IapPayWaresId", 			"爱贝支付商品编号"		);
			typeNameDic.Add (	"IapPayPrivateKey", 		"爱贝支付私钥"			);
			typeNameDic.Add (	"IapPayPublicKey", 			"爱贝支付公钥"			);
			typeNameDic.Add (	"X7AppKey", 				"小七支付Key"			);
			typeNameDic.Add (	"X7RSA", 					"小七支付RSA公钥"		);
			typeNameDic.Add (	"ReyunKey", 				"热云统计Key"			);
			typeNameDic.Add (	"VungleAppId", 				"Vungle视频ID"		);
			typeNameDic.Add (	"VunglePlaceId", 			"Vungle视频广告位"	);
			typeNameDic.Add (	"LanmeiAppId", 				"蓝莓视频ID"			);
			typeNameDic.Add (	"DtseaSlotId", 				"锁屏广告位"			);

			#endregion
		}

		#region 这里是各种key的中文备注

		public static string GetDataTypeName(string key){
			if (typeNameDic == null || typeNameDic.Count == 0) {
				InitTypeName ();
			}
			if (typeNameDic.ContainsKey (key)) {
				return typeNameDic [key];
			}
			return key;
		}
		#endregion
	}
}