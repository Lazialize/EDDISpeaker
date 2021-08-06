using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace AITalkAPI
{
    internal static class AITalkCore
    {
        public enum EventReason
        {
            TextBufferFull = 101,
            TextBufferFlush = 102,
            TextBufferClose = 103,
            RawBufferFull = 201,
            RawBufferFlush = 202,
            RawBufferClose = 203,
            PhoneticLabel = 301,
            Bookmark = 302,
            AutoBookmark = 303
        }

        public enum Result
        {
            Success = 0,
            InternalError = -1,
            Unsupported = -2,
            InvalidArgument = -3,
            WaitTimeout = -4,
            NotInitialized = -10,
            AlreadyInitialized = 10,
            NotLoaded = -11,
            AlreadyLoaded = 11,
            Insufficient = -20,
            PartiallyRegistered = 21,
            LicenseAbsent = -100,
            LicenseExpired = -101,
            LicenseRejected = -102,
            TooManyJobs = -201,
            InvalidJobId = -202,
            JobBusy = -203,
            NoMoreData = 204,
            OutOfMemory = -206,
            FileNotFound = -1001,
            PathNotFound = -1002,
            ReadFault = -1003,
            CountLimit = -1004,
            UserDictionaryLocked = -1011,
            UserDictionaryNoEntry = -1012
        }

        public enum Status
        {
            WrongState = -1,
            InProgress = 10,
            StillRunning = 11,
            Done = 12
        }

        public enum JobInOut
        {
            PlainToWave = 11,
            KanaToWave = 12,
            JeitaToWave = 13,
            PlainToKana = 21,
            KanaToJeita = 32
        }

        [Flags]
        public enum ExtendFormat
        {
            None = 0x0,
            JeitaRuby = 0x1,
            AutoBookmark = 0x10
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct Config
        {
            public int VoiceDbSampleRate;

            [MarshalAs(UnmanagedType.LPStr)]
            public string VoiceDbDirectory;

            public int TimeoutMilliseconds;

            [MarshalAs(UnmanagedType.LPStr)]
            public string LicensePath;

            [MarshalAs(UnmanagedType.LPStr)]
            public string AuthenticateCodeSeed;

            public int ReservedZero;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct JobParam
        {
            public JobInOut ModeInOut;
            public IntPtr UserData;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct TtsParam
        {
            public const int VoiceNameLength = 80;

            public int Size;

            public TextBufferCallbackType TextBufferCallback;

            public RawBufferCallbackType RawBufferCallback;

            public TtsEventCallbackType TtsEventCallback;

            public int TextBufferCapacityInBytes;

            public int RawBufferCapacityInBytes;

            public float Volume;

            public int PauseBegin;

            public int PauseTerm;

            public ExtendFormat ExtendFormatFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = VoiceNameLength)]
            public string VoiceName;

            public JeitaParam Jeita;

            public int NumberOfSpeakers;

            public int ReservedZero;

            [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
            public struct JeitaParam
            {
                public const int ControlLength = 12;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = VoiceNameLength)]
                public string FemaleName;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = VoiceNameLength)]
                public string MaleName;

                public int PauseMiddle;

                public int PauseLong;

                public int PauseSentence;

                /// <summary>
                /// JEITA TT-6004を参照せよ
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ControlLength)]
                public string Control;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
            [DataContract]
            public class SpeakerParam
            {
                [DataMember]
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = VoiceNameLength)]
                public string VoiceName;

                [DataMember]
                public float Volume;

                [DataMember]
                public float Speed;

                [DataMember]
                public float Pitch;

                [DataMember]
                public float Range;

                [DataMember]
                public int PauseMiddle;

                [DataMember]
                public int PauseLong;

                [DataMember]
                public int PauseSentence;

                [DataMember]
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = VoiceNameLength)]
                public string StyleRate;
            }

            public delegate int TextBufferCallbackType(EventReason reason, int job_id, IntPtr user_data);

            public delegate int RawBufferCallbackType(EventReason reason, int job_id, long tick, IntPtr user_data);

            public delegate int TtsEventCallbackType(EventReason reason, int job_id, long tick, string name, IntPtr user_data);
        }
    }
}
