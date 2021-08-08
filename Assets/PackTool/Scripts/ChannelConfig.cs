using System.Collections.Generic;
using UnityEngine;
using System;


namespace TomatoJoy.Data
{
    [SerializeField]
    public class ChannelConfig
    {
        public List<Channel> channels;
    }

    [Serializable]
    public class Channel
    {
        public string channelId;
        public ChannelInfo channelInfo;
        public ChannelData channelData;
        public ChannelMeta channelMeta;
    }

    [Serializable]
    public class ChannelInfo
    {
        public string ChannelCN;
        public string ProductName;
        public string BundleId;
        public string BundleVersion;
        public string ShortBundleVersion;
        public string BundleVersionCode;
        public string Icon;
        public string Splash;
        public string DefineSymbol;
        public string AndroidLib;
    }

    [Serializable]
    public class ChannelData
    {

    }

    [Serializable]
    public class ChannelMeta
    {

    }
}