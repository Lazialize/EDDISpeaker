using System;
using System.Collections.Generic;
using System.Text;

namespace EDDISpeaker
{
    class Config
    {
        public class VoiceroidConfig
        {
            public string AuthenticationCode { get; set; }
            public string InstallationPath { get; set; }

            public string PhraseDictionaryPath { get; set; }
            public string WordDictionaryPath { get; set; }
            public string SymbolDictionaryPath { get; set; }

            public double Volume { get; set; }
            public double Tempo { get; set; }
            public double Pitch { get; set; }
            public double Intonation { get; set; }

            public int ShortPause { get; set; }
            public int LongPause { get; set; }
            public int SentencePause { get; set; }
        }

        public VoiceroidConfig VoiceroidSettings { get; set; } = new VoiceroidConfig()
        {
            AuthenticationCode = null,
            InstallationPath = null,

            PhraseDictionaryPath = null,
            WordDictionaryPath = null,
            SymbolDictionaryPath = null,

            Volume = 1,
            Tempo = 1,
            Pitch = 1,
            Intonation = 1,

            ShortPause = 150,
            LongPause = 370,
            SentencePause = 800
        };
    }
}
