using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceRSS_SDK;

namespace EnglishLearning.ExtendClasses
{
    class VoiceAPI
    {
        public byte[] UploadVoice(string engText)
        {
            string apiKey = "6319e58e8ef743e39bfca3a49bb1a3ec";
            bool isSSL = false;
            string text = engText;
            string lang = Languages.English_UnitedStates;
            VoiceParameters voiceParams = new VoiceParameters(text, lang)
            {
                AudioCodec = AudioCodec.MP3,
                AudioFormat = AudioFormat.Format_44KHZ.AF_44khz_16bit_stereo,
                IsBase64 = false,
                IsSsml = false,
                SpeedRate = 0
            };
            VoiceProvider voiceProvider = new VoiceProvider(apiKey, isSSL);
            byte[] voice = new byte[0];
            try
            {
                voice = voiceProvider.Speech<byte[]>(voiceParams);
            }
            catch (Exception ex)
            {
            }
            return voice;
        }
    }
}