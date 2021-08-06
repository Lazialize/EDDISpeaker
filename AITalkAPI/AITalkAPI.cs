using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AITalkAPI
{
    internal static class AITalkAPI
    {
        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_Init@4")]
        public static extern AITalkCore.Result Init([In] ref AITalkCore.Config config);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_End@0")]
        public static extern AITalkCore.Result End();

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_LangLoad@4")]
        public static extern AITalkCore.Result LangLoad(string language_name);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_LangClear@0")]
        public static extern AITalkCore.Result LangClear();

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_VoiceLoad@4")]
        public static extern AITalkCore.Result VoiceLoad(string voice_name);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_VoiceClear@0")]
        public static extern AITalkCore.Result VoiceClear();

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_ReloadPhraseDic@4")]
        public static extern AITalkCore.Result ReloadPhraseDic(string dictionary_path);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_ReloadSymbolDic@4")]
        public static extern AITalkCore.Result ReloadSymbolDic(string dictionary_path);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_ReloadWordDic@4")]
        public static extern AITalkCore.Result ReloadWordDic(string dictionary_path);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_GetParam@8")]
        public static extern AITalkCore.Result GetParam(IntPtr param, ref int written_bytes);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_SetParam@4")]
        public static extern AITalkCore.Result SetParam(IntPtr param);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_TextToKana@12")]
        public static extern AITalkCore.Result TextToKana(out int job_id, [In] ref AITalkCore.JobParam job_param, [In] byte[] text);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_GetKana@20")]
        public static extern AITalkCore.Result GetKana(int job_id, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int buffer_capacity, out int read_bytes, out int position);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_CloseKana@8")]
        public static extern AITalkCore.Result CloseKana(int job_id, int zero = 0);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_TextToSpeech@12")]
        public static extern AITalkCore.Result TextToSpeech(out int job_id, [In] ref AITalkCore.JobParam job_param, string text);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_GetData@16")]
        public static extern AITalkCore.Result GetData(int job_id, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int buffer_capacity, out int read_samples);

        [DllImport("aitalked.dll", EntryPoint = "_AITalkAPI_CloseSpeech@8")]
        public static extern AITalkCore.Result CloseSpeech(int job_id, int zero = 0);
    }
}
