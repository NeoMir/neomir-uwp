using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

namespace NeoMir.Classes
{
    public class mySpeechRecognition
    {
        private CoreDispatcher _dispatcher;
        SpeechRecognizer recognizer;
        public mySpeechRecognition()
        {
        }


        async public Task<String> GetStringFromSpeechRecognition()
        {
            String str = "";
            var recognizer = new SpeechRecognizer();
            await recognizer.CompileConstraintsAsync();
            var result = await recognizer.RecognizeAsync();
            if (result != null)
            {
                str = result.Text.ToString();
            }
            return str;
        }
    }
}
