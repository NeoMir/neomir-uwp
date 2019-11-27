using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes.Com
{
    public class FaceCollector
    {
        private const string FileToReadName = "faces.txt";
        private static volatile FaceCollector instance;
        private static object syncRoot = new object();
        private StorageFile file;
        private StorageFileQueryResult query;
        private DateTimeOffset lastModification;
        public delegate void FaceCollectedHandler(Face gesture);
        private Dictionary<Page, Action<Face>> pageEventDico;
        private BasicProperties prop;
        private string lastMessage;

        /// <summary>
        /// Gets an Instance of the classe if the it's already existing
        /// </summary>
        /// <value>LoggingHandler</value>
        public static FaceCollector Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new FaceCollector();
                        }
                    }
                }
                return instance;
            }
        }

        private FaceCollector()
        {
            GetQuery();
            pageEventDico = new Dictionary<Page, Action<Face>>();
        }

        // Lance le file watching
        private async Task GetQuery()
        {
            StorageFolder folder = KnownFolders.PicturesLibrary;
            file = await folder.GetFileAsync(FileToReadName);
            while (true)
            {
                try
                {
                    await GetMessage(null, null);
                    await Task.Delay(1000);
                }
                catch
                {
                }
            };
        }

        // Ouvre le fichier 
        private async Task GetMessage(Windows.Storage.Search.IStorageQueryResultBase sender, object args)
        {
            string text = await FileIO.ReadTextAsync(file);
            lastMessage = lastMessage == default(string) ? text : lastMessage;
            if (text != lastMessage)
            {
                lastMessage = text;
                Page current = FrameManager.GetCurrentPage();
                if (pageEventDico.ContainsKey(current))
                {
                    int index = text.IndexOf('-');
                    if (index > 0)
                    {
                        pageEventDico[current].Invoke(new Face(text.Substring(0, index)));
                    }
                }
            }
        }

        private int GetTimeElapsed(DateTimeOffset current, DateTimeOffset last)
        {
            int currentMilli = current.Millisecond + (current.Second * 100) + (current.Minute * 60 * 100);
            int lastMilli = last.Millisecond + (last.Second * 100) + (last.Minute * 60 * 100);
            return currentMilli - lastMilli;
        }

        // Methode a utiliser pour s'enregistrer à l'évènement de reconnaissance faciale
        public void RegisterToFace(Page page, Action<Face> action)
        {
            if (!pageEventDico.ContainsKey(page))
            {
                pageEventDico.Add(page, action);
            }
        }
    }
}
