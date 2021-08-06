using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace AITalkAPI
{
    public static class AITalkWrapper
    {
        /// <summary>
        /// AITalkを初期化する
        /// </summary>
        /// <param name="installDirectory">VOICEROID2のインストールディレクトリ</param>
        /// <param name="authenticationCode">認証コード</param>
        public static void Initialize(string installDirectory, string authenticationCode)
        {
            Finish();

            // aitalked.dllをロードするために
            // DLLの探索パスをVOICEROID2のディレクトリに変更する
            if ((InstallDirectory != null) && (InstallDirectory != installDirectory))
            {
                throw new AITalkException($"インストールディレクトリを変更して再び初期化することはできません。");
            }
            InstallDirectory = installDirectory;
            SetDllDirectory(InstallDirectory);

            // AITalkを初期化する
            AITalkCore.Config config;
            config.VoiceDbSampleRate = VoiceSampleRate;
            config.VoiceDbDirectory = $"{InstallDirectory}\\Voice";
            config.TimeoutMilliseconds = TimeoutMilliseconds;
            config.LicensePath = $"{InstallDirectory}\\aitalk.lic";
            config.AuthenticateCodeSeed = authenticationCode;
            config.ReservedZero = 0;
            var result = AITalkCore.Result.Success;
            try
            {
                result = AITalkAPI.Init(ref config);
            }
            catch (Exception e)
            {
                throw new AITalkException($"AITalkの初期化に失敗しました。", e);
            }
            if (result != AITalkCore.Result.Success)
            {
                throw new AITalkException($"AITalkの初期化に失敗しました。", result);
            }
            IsInitialized = true;
        }

        /// <summary>
        /// AITalkを終了する
        /// </summary>
        public static void Finish()
        {
            if (IsInitialized == true)
            {
                IsInitialized = false;
                AITalkAPI.End();
            }
            CurrentLanguage = null;
            CurrentVoice = null;
        }

        /// <summary>
        /// 言語ライブラリの一覧。
        /// インストールディレクトリのLangディレクトリの中にあるフォルダ名から生成される。
        /// </summary>
        public static string[] LanguageList
        {
            get
            {
                List<string> result = new List<string>();
                try
                {
                    foreach (string path in Directory.GetDirectories($"{InstallDirectory}\\Lang"))
                    {
                        result.Add(Path.GetFileName(path));
                    }
                }
                catch (Exception) { }
                result.Sort(StringComparer.InvariantCultureIgnoreCase);
                return result.ToArray();
            }
        }

        /// <summary>
        /// ボイスライブラリの一覧。
        /// インストールディレクトリのVoiceディレクトリの中にあるフォルダ名から生成される。
        /// </summary>
        public static string[] VoiceDbList
        {
            get
            {
                List<string> result = new List<string>();
                try
                {
                    foreach (string path in Directory.GetDirectories($"{InstallDirectory}\\Voice"))
                    {
                        result.Add(Path.GetFileName(path));
                    }
                }
                catch (Exception) { }
                result.Sort(StringComparer.InvariantCultureIgnoreCase);
                return result.ToArray();
            }
        }

        /// <summary>
        /// 言語ライブラリを読み込む
        /// </summary>
        /// <param name="languageName">言語名</param>
        public static void LoadLanguage(string languageName)
        {
            if (languageName == CurrentLanguage)
            {
                return;
            }
            // 言語の設定をする際はカレントディレクトリを一時的にVOICEROID2のインストールディレクトリに変更する
            // それ以外ではLangLoad()はエラーを返す
            string current_directory = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.SetCurrentDirectory(InstallDirectory);
            CurrentLanguage = null;
            AITalkCore.Result result;
            result = AITalkAPI.LangClear();
            if ((result == AITalkCore.Result.Success) || (result == AITalkCore.Result.NotLoaded))
            {
                result = AITalkAPI.LangLoad($"{InstallDirectory}\\Lang\\{languageName}");
            }
            System.IO.Directory.SetCurrentDirectory(current_directory);
            if (result != AITalkCore.Result.Success)
            {
                throw new AITalkException($"言語'{languageName}'の読み込みに失敗しました。", result);
            }
            CurrentLanguage = languageName;
        }

        /// <summary>
        /// フレーズ辞書を読み込む
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public static void ReloadPhraseDictionary(string path)
        {
            AITalkAPI.ReloadPhraseDic(null);
            if (path == null)
            {
                return;
            }
            AITalkCore.Result result;
            result = AITalkAPI.ReloadPhraseDic(path);
            if (result == AITalkCore.Result.UserDictionaryNoEntry)
            {
                AITalkAPI.ReloadPhraseDic(null);
            }
            else if (result != AITalkCore.Result.Success)
            {
                throw new AITalkException($"フレーズ辞書'{path}'の読み込みに失敗しました。", result);
            }
        }

        /// <summary>
        /// 単語辞書を読み込む
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public static void ReloadWordDictionary(string path)
        {
            AITalkAPI.ReloadWordDic(null);
            if (path == null)
            {
                return;
            }
            AITalkCore.Result result;
            result = AITalkAPI.ReloadWordDic(path);
            if (result == AITalkCore.Result.UserDictionaryNoEntry)
            {
                AITalkAPI.ReloadWordDic(null);
            }
            else if (result != AITalkCore.Result.Success)
            {
                throw new AITalkException($"単語辞書'{path}'の読み込みに失敗しました。", result);
            }
        }

        /// <summary>
        /// 記号ポーズ辞書を読み込む
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public static void ReloadSymbolDictionary(string path)
        {
            AITalkAPI.ReloadSymbolDic(null);
            if (path == null)
            {
                return;
            }
            AITalkCore.Result result;
            result = AITalkAPI.ReloadSymbolDic(path);
            if (result == AITalkCore.Result.UserDictionaryNoEntry)
            {
                AITalkAPI.ReloadSymbolDic(null);
            }
            else if (result != AITalkCore.Result.Success)
            {
                throw new AITalkException($"記号ポーズ辞書'{path}'の読み込みに失敗しました。", result);
            }
        }

        /// <summary>
        /// ボイスライブラリを読み込む
        /// </summary>
        /// <param name="VoiceDbName">ボイスライブラリ名</param>
        public static void LoadVoice(string VoiceDbName)
        {
            if (VoiceDbName == CurrentVoice)
            {
                return;
            }

            CurrentVoice = null;
            AITalkAPI.VoiceClear();
            if (VoiceDbName == null)
            {
                return;
            }
            AITalkCore.Result result;
            result = AITalkAPI.VoiceLoad(VoiceDbName);
            if (result != AITalkCore.Result.Success)
            {
                throw new AITalkException($"ボイスライブラリ'{VoiceDbName}'の読み込みに失敗しました。", result);
            }

            // パラメータを読み込む
            GetParameters(out var ttsParam, out var speakerParams);
            ttsParam.TextBufferCallback = TextBufferCallback;
            ttsParam.RawBufferCallback = RawBufferCallback;
            ttsParam.TtsEventCallback = TtsEventCallback;
            ttsParam.PauseBegin = 0;
            ttsParam.PauseTerm = 0;
            ttsParam.ExtendFormatFlags = AITalkCore.ExtendFormat.JeitaRuby | AITalkCore.ExtendFormat.AutoBookmark;
            Parameter = new AITalkParameter(VoiceDbName, ttsParam, speakerParams);

            CurrentVoice = VoiceDbName;
        }

        /// <summary>
        /// パラメータを取得する
        /// </summary>
        /// <param name="ttsParam">パラメータ(話者パラメータを除く)</param>
        /// <param name="speakerParams">話者パラメータ</param>
        private static void GetParameters(out AITalkCore.TtsParam ttsParam, out AITalkCore.TtsParam.SpeakerParam[] speakerParams)
        {
            // パラメータを格納するのに必要なバッファサイズを取得する
            AITalkCore.Result result;
            int size = 0;
            result = AITalkAPI.GetParam(IntPtr.Zero, ref size);
            if ((result != AITalkCore.Result.Insufficient) || (size < Marshal.SizeOf<AITalkCore.TtsParam>()))
            {
                throw new AITalkException("動作パラメータの長さの取得に失敗しました。", result);
            }

            IntPtr ptr = Marshal.AllocCoTaskMem(size);
            try
            {
                // パラメータを読み取る
                Marshal.WriteInt32(ptr, (int)Marshal.OffsetOf<AITalkCore.TtsParam>("Size"), size);
                result = AITalkAPI.GetParam(ptr, ref size);
                if (result != AITalkCore.Result.Success)
                {
                    throw new AITalkException("動作パラメータの取得に失敗しました。", result);
                }
                ttsParam = Marshal.PtrToStructure<AITalkCore.TtsParam>(ptr);

                // 話者のパラメータを読み取る
                speakerParams = new AITalkCore.TtsParam.SpeakerParam[ttsParam.NumberOfSpeakers];
                for (int index = 0; index < speakerParams.Length; index++)
                {
                    IntPtr speakerPtr = IntPtr.Add(ptr, Marshal.SizeOf<AITalkCore.TtsParam>() + Marshal.SizeOf<AITalkCore.TtsParam.SpeakerParam>() * index);
                    speakerParams[index] = Marshal.PtrToStructure<AITalkCore.TtsParam.SpeakerParam>(speakerPtr);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        /// <summary>
        /// パラメータを設定する。
        /// param.Sizeおよびparam.NumberOfSpeakersは自動的に設定される。
        /// </summary>
        /// <param name="ttsParam">パラメータ(話者パラメータを除く)</param>
        /// <param name="speakerParams">話者パラメータ</param>
        private static void SetParameters(AITalkCore.TtsParam ttsParam, AITalkCore.TtsParam.SpeakerParam[] speakerParams)
        {
            // パラメータを格納するバッファを確保する
            int size = Marshal.SizeOf<AITalkCore.TtsParam>() + Marshal.SizeOf<AITalkCore.TtsParam.SpeakerParam>() * speakerParams.Length;
            IntPtr ptr = Marshal.AllocCoTaskMem(size);
            try
            {
                // パラメータを設定する
                ttsParam.Size = size;
                ttsParam.NumberOfSpeakers = speakerParams.Length;
                Marshal.StructureToPtr<AITalkCore.TtsParam>(ttsParam, ptr, false);
                for (int index = 0; index < speakerParams.Length; index++)
                {
                    IntPtr speakerPtr = IntPtr.Add(ptr, Marshal.SizeOf<AITalkCore.TtsParam>() + Marshal.SizeOf<AITalkCore.TtsParam.SpeakerParam>() * index);
                    Marshal.StructureToPtr<AITalkCore.TtsParam.SpeakerParam>(speakerParams[index], speakerPtr, false);
                }
                AITalkCore.Result result;
                result = AITalkAPI.SetParam(ptr);
                if (result != AITalkCore.Result.Success)
                {
                    throw new AITalkException("動作パラメータの設定に失敗しました。", result);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        /// <summary>
        /// パラメータが更新されていれば反映する
        /// </summary>
        private static void UpdateParameter()
        {
            if (Parameter.IsParameterChanged == true)
            {
                // パラメータを更新する
                SetParameters(Parameter.TtsParam, Parameter.SpeakerParameters);
                Parameter.IsParameterChanged = false;
            }
        }

        /// <summary>
        /// テキストを読み仮名に変換する
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="timeout">タイムアウト[ms]。0以下はタイムアウト無しで待ち続ける。</param>
        /// <returns>読み仮名文字列</returns>
        public static string TextToKana(string text, int timeout = 0)
        {
            UpdateParameter();

            // ShiftJISに変換する
            UnicodeToShiftJis(text, out byte[] shiftjisBytes, out int[] shiftjisPositions);

            // コールバックメソッドとの同期オブジェクトを用意する
            KanaJobData jobData = new KanaJobData();
            jobData.BufferCapacity = 0x1000;
            jobData.Output = new List<byte>();
            jobData.CloseEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
            GCHandle gcHandle = GCHandle.Alloc(jobData);
            try
            {
                // 変換を開始する
                AITalkCore.JobParam jobParam;
                jobParam.ModeInOut = AITalkCore.JobInOut.PlainToKana;
                jobParam.UserData = GCHandle.ToIntPtr(gcHandle);
                AITalkCore.Result result;
                result = AITalkAPI.TextToKana(out int jobId, ref jobParam, shiftjisBytes);
                if (result != AITalkCore.Result.Success)
                {
                    throw new AITalkException($"仮名変換が開始できませんでした。[{string.Join(",", shiftjisBytes)}]", result);
                }

                // 変換の終了を待つ
                // timeoutで与えられた時間だけ待つ
                bool respond;
                respond = jobData.CloseEvent.WaitOne((0 < timeout) ? timeout : -1);

                // 変換を終了する
                result = AITalkAPI.CloseKana(jobId);
                if (respond == false)
                {
                    throw new AITalkException("仮名変換がタイムアウトしました。");
                }
                else if (result != AITalkCore.Result.Success)
                {
                    throw new AITalkException("仮名変換が正常に終了しませんでした。", result);
                }
            }
            finally
            {
                gcHandle.Free();
            }

            // 変換結果に含まれるIrq MARKのバイト位置を文字位置へ置き換える
            Encoding encoding = Encoding.GetEncoding(932);
            return ReplaceIrqMark(encoding.GetString(jobData.Output.ToArray()), shiftjisPositions);
        }

        /// <summary>
        /// UTF-16からShiftJISに文字列を変換し、文字位置の変換テーブルを生成する。
        /// 変換後のShiftJIS文字列と変換テーブルにはヌル終端の分の要素も含まれる。
        /// </summary>
        /// <param name="unicodeString">UTF-16文字列</param>
        /// <param name="shiftjisString">ShiftJIS文字列</param>
        /// <param name="shiftjis_positions">ShiftJISのバイト位置と文字位置の変換テーブル</param>
        private static void UnicodeToShiftJis(string unicodeString, out byte[] shiftjisString, out int[] shiftjisPositions)
        {
            // 文字位置とUTF-16上でのワード位置の変換テーブルを取得し、
            // ShiftJIS上でのバイト位置とUTF-16上でのワード位置の変換テーブルを計算する
            Encoding encoding = Encoding.GetEncoding(932);
            byte[] shiftjisStringInternal = encoding.GetBytes(unicodeString);
            int shiftjisLength = shiftjisStringInternal.Length;
            shiftjisPositions = new int[shiftjisLength + 1];
            char[] unicodeCharArray = unicodeString.ToArray();
            int[] unicodeIndexes = StringInfo.ParseCombiningCharacters(unicodeString);
            int charCount = unicodeIndexes.Length;
            int shiftjisIndex = 0;
            for (int charIndex = 0; charIndex < charCount; charIndex++)
            {
                int unicodeIndex = unicodeIndexes[charIndex];
                int unicodeCount = (((charIndex + 1) < charCount) ? unicodeIndexes[charIndex + 1] : unicodeString.Length) - unicodeIndex;
                int shiftjisCount = encoding.GetByteCount(unicodeCharArray, unicodeIndex, unicodeCount);
                for (int offset = 0; offset < shiftjisCount; offset++)
                {
                    shiftjisPositions[shiftjisIndex + offset] = charIndex;
                }
                shiftjisIndex += shiftjisCount;
            }
            shiftjisPositions[shiftjisLength] = charCount;

            // ヌル終端を付け加える
            shiftjisString = new byte[shiftjisLength + 1];
            Buffer.BlockCopy(shiftjisStringInternal, 0, shiftjisString, 0, shiftjisLength);
            shiftjisString[shiftjisLength] = 0;
        }

        /// <summary>
        /// Irq MARKによる文節位置を実際の文字位置に置き換える
        /// </summary>
        /// <param name="input">文字列</param>
        /// <param name="shiftjisPositions">ShiftJISのバイト位置と文字位置の変換テーブル</param>
        /// <returns>変換された文字列</returns>
        private static string ReplaceIrqMark(string input, int[] shiftjisPositions)
        {
            StringBuilder output = new StringBuilder();
            int shiftjisLength = shiftjisPositions.Length;
            int index = 0;
            const string StartOfIrqMark = "(Irq MARK=_AI@";
            const string EndOfIrqMask = ")";
            while (true)
            {
                int startPos = input.IndexOf(StartOfIrqMark, index);
                if (startPos < 0)
                {
                    output.Append(input, index, input.Length - index);
                    break;
                }
                startPos += StartOfIrqMark.Length;
                output.Append(input, index, startPos - index);
                int endPos = input.IndexOf(EndOfIrqMask, startPos);
                if (endPos < 0)
                {
                    output.Append(input, index, input.Length - startPos);
                    break;
                }
                if (int.TryParse(input.Substring(startPos, endPos - startPos), out int shiftjisIndex) == false)
                {
                    throw new AITalkException("文節位置の取得に失敗しました。");
                }
                if ((shiftjisIndex < 0) || (shiftjisLength <= shiftjisIndex))
                {
                    throw new AITalkException("文節位置の特定に失敗しました。");
                }
                output.Append(shiftjisPositions[shiftjisIndex]);
                output.Append(EndOfIrqMask);
                index = endPos + EndOfIrqMask.Length;
            }
            return output.ToString();
        }

        /// <summary>
        /// 読み仮名変換時のコールバックメソッド
        /// </summary>
        /// <param name="reason">呼び出し要因</param>
        /// <param name="jobId">ジョブID</param>
        /// <param name="userData">ユーザーデータ(KanaJobDataへのポインタ)</param>
        /// <returns>ゼロを返す</returns>
        private static int TextBufferCallback(AITalkCore.EventReason reason, int jobId, IntPtr userData)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userData);
            KanaJobData jobData = gcHandle.Target as KanaJobData;
            if (jobData == null)
            {
                return 0;
            }

            // 変換できた分だけGetKana()で読み取ってjob_dataのバッファに格納する
            int bufferCapacity = jobData.BufferCapacity;
            byte[] buffer = new byte[bufferCapacity];
            AITalkCore.Result result;
            int readBytes;
            do
            {
                result = AITalkAPI.GetKana(jobId, buffer, bufferCapacity, out readBytes, out _);
                if (result != AITalkCore.Result.Success)
                {
                    break;
                }
                jobData.Output.AddRange(new ArraySegment<byte>(buffer, 0, readBytes));
            }
            while ((bufferCapacity - 1) <= readBytes);
            if (reason == AITalkCore.EventReason.TextBufferClose)
            {
                jobData.CloseEvent.Set();
            }
            return 0;
        }

        /// <summary>
        /// 読み仮名を読み上げてWAVEファイルをストリームに出力する。
        /// なお、ストリームへの書き込みは変換がすべて終わった後に行われる。
        /// </summary>
        /// <param name="kana">読み仮名</param>
        /// <param name="waveStream">WAVEファイルの出力先ストリーム</param>
        /// <param name="timeout">タイムアウト[ms]。0以下はタイムアウト無しで待ち続ける。</param>
        public static void KanaToSpeech(string kana, Stream waveStream, int timeout = 0)
        {
            UpdateParameter();

            // コールバックメソッドとの同期オブジェクトを用意する
            SpeechJobData jobData = new SpeechJobData();
            jobData.BufferCapacity = 176400;
            jobData.Output = new List<byte>();
            jobData.EventData = new List<TtsEventData>();
            jobData.CloseEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
            GCHandle gcHandle = GCHandle.Alloc(jobData);
            try
            {
                // 変換を開始する
                AITalkCore.JobParam jobParam;
                jobParam.ModeInOut = AITalkCore.JobInOut.KanaToWave;
                jobParam.UserData = GCHandle.ToIntPtr(gcHandle);
                AITalkCore.Result result;
                result = AITalkAPI.TextToSpeech(out int job_id, ref jobParam, kana);
                if (result != AITalkCore.Result.Success)
                {
                    throw new AITalkException("音声変換が開始できませんでした。", result);
                }

                // 変換の終了を待つ
                // timeoutで与えられた時間だけ待つ
                bool respond;
                respond = jobData.CloseEvent.WaitOne((0 < timeout) ? timeout : -1);

                // 変換を終了する
                result = AITalkAPI.CloseSpeech(job_id);
                if (respond == false)
                {
                    throw new AITalkException("音声変換がタイムアウトしました。");
                }
                else if (result != AITalkCore.Result.Success)
                {
                    throw new AITalkException("音声変換が正常に終了しませんでした。", result);
                }
            }
            finally
            {
                gcHandle.Free();
            }

            // TTSイベントをJSONに変換する
            // 変換後の文字列にヌル終端がてら4の倍数の長さになるようパディングを施す
            MemoryStream eventStream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(List<TtsEventData>));
            serializer.WriteObject(eventStream, jobData.EventData);
            int padding = 4 - ((int)eventStream.Length % 4);
            for (int cnt = 0; cnt < padding; cnt++)
            {
                eventStream.WriteByte(0x0);
            }
            byte[] event_json = eventStream.ToArray();

            // データをWAVE形式で出力する
            // phonチャンクとしてTTSイベントを埋め込む
            byte[] data = jobData.Output.ToArray();
            var writer = new BinaryWriter(waveStream);
            writer.Write(new byte[4] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' });
            writer.Write(44 + event_json.Length + data.Length);
            writer.Write(new byte[4] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' });
            writer.Write(new byte[4] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });
            writer.Write(16);
            writer.Write((short)0x1);
            writer.Write((short)1);
            writer.Write(VoiceSampleRate);
            writer.Write(2 * VoiceSampleRate);
            writer.Write((short)2);
            writer.Write((short)16);
            writer.Write(new byte[4] { (byte)'p', (byte)'h', (byte)'o', (byte)'n' });
            writer.Write(event_json.Length);
            writer.Write(event_json);
            writer.Write(new byte[4] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' });
            writer.Write(data.Length);
            writer.Write(data);
        }

        /// <summary>
        /// 音声変換時のデータコールバックメソッド
        /// </summary>
        /// <param name="reason">呼び出し要因</param>
        /// <param name="jobId">ジョブID</param>
        /// <param name="tick">時刻[ms]</param>
        /// <param name="userData">ユーザーデータ(SpeechJobDataへのポインタ)</param>
        /// <returns>ゼロを返す</returns>
        private static int RawBufferCallback(AITalkCore.EventReason reason, int jobId, long tick, IntPtr userData)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userData);
            SpeechJobData jobData = gcHandle.Target as SpeechJobData;
            if (jobData == null)
            {
                return 0;
            }

            // 変換できた分だけGetData()で読み取ってjob_dataのバッファに格納する
            int bufferCapacity = jobData.BufferCapacity;
            byte[] buffer = new byte[2 * bufferCapacity];
            AITalkCore.Result result;
            int readSamples;
            do
            {
                result = AITalkAPI.GetData(jobId, buffer, bufferCapacity, out readSamples);
                if (result != AITalkCore.Result.Success)
                {
                    break;
                }
                jobData.Output.AddRange(new ArraySegment<byte>(buffer, 0, 2 * readSamples));
            }
            while ((bufferCapacity - 1) <= readSamples);
            if (reason == AITalkCore.EventReason.RawBufferClose)
            {
                jobData.CloseEvent.Set();
            }
            return 0;
        }

        /// <summary>
        /// 音声変換時のイベントコールバックメソッド
        /// </summary>
        /// <param name="reason">呼び出し要因</param>
        /// <param name="jobId">ジョブID</param>
        /// <param name="tick">時刻[ms]</param>
        /// <param name="name">イベントの値</param>
        /// <param name="userData">ユーザーデータ(SpeechJobDataへのポインタ)</param>
        /// <returns>ゼロを返す</returns>
        private static int TtsEventCallback(AITalkCore.EventReason reason, int jobId, long tick, string name, IntPtr userData)
        {
            GCHandle gcHandle = GCHandle.FromIntPtr(userData);
            SpeechJobData jobData = gcHandle.Target as SpeechJobData;
            if (jobData == null)
            {
                return 0;
            }
            switch (reason)
            {
                case AITalkCore.EventReason.PhoneticLabel:
                case AITalkCore.EventReason.Bookmark:
                case AITalkCore.EventReason.AutoBookmark:
                    jobData.EventData.Add(new TtsEventData(tick, name, reason));
                    break;
            }
            return 0;
        }

        /// <summary>
        /// パラメータ
        /// </summary>
        public static AITalkParameter Parameter { get; private set; }

        /// <summary>
        /// インストールディレクトリ
        /// </summary>
        public static string InstallDirectory { get; private set; }

        /// <summary>
        /// 初期化が成功したならtrueを返す
        /// </summary>
        public static bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// 言語ライブラリが読み込まれているならtrueを返す
        /// </summary>
        public static bool IsLanguageLoaded { get { return CurrentLanguage != null; } }

        /// <summary>
        /// 読み込まれている言語ライブラリ名
        /// </summary>
        public static string CurrentLanguage { get; private set; }

        /// <summary>
        /// ボイスライブラリが読み込まれているならtrueを返す
        /// </summary>
        public static bool IsVoiceLoaded { get { return CurrentVoice != null; } }

        /// <summary>
        /// 読み込まれているボイスライブラリ名
        /// </summary>
        public static string CurrentVoice { get; private set; }

        /// <summary>
        /// 仮名変換のジョブを管理するクラス
        /// </summary>
        private class KanaJobData
        {
            public int BufferCapacity;
            public List<byte> Output;
            public EventWaitHandle CloseEvent;
        }

        /// <summary>
        /// 音声変換のジョブを管理するクラス
        /// </summary>
        private class SpeechJobData
        {
            public int BufferCapacity;
            public List<byte> Output;
            public List<TtsEventData> EventData;
            public EventWaitHandle CloseEvent;
        }

        /// <summary>
        /// TTSイベントのデータを格納する構造体
        /// </summary>
        [DataContract]
        public struct TtsEventData
        {
            [DataMember]
            public long Tick;

            [DataMember]
            public string Value;

            [DataMember]
            public string Type;

            internal TtsEventData(long tick, string value, AITalkCore.EventReason reason)
            {
                Tick = tick;
                Value = value;
                switch (reason)
                {
                    case AITalkCore.EventReason.PhoneticLabel:
                        Type = "Phonetic";
                        break;
                    case AITalkCore.EventReason.Bookmark:
                        Type = "Bookmark";
                        break;
                    case AITalkCore.EventReason.AutoBookmark:
                        Type = "AutoBookmark";
                        break;
                    default:
                        Type = "";
                        break;
                }
            }
        }

        /// <summary>
        /// ボイスライブラリのサンプルレート[Hz]
        /// </summary>
        private const int VoiceSampleRate = 44100;

        /// <summary>
        /// AITalkのタイムアウト[ms]
        /// </summary>
        private const int TimeoutMilliseconds = 1000;

        [DllImport("Kernel32.dll")]
        private static extern bool SetDllDirectory(string lpPathName);
    }
}
