using NeoMir.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes.Com
{
    public class GlobalMessageManager
    {
        private static volatile GlobalMessageManager instance;
        private static object syncRoot = new object();
        private StorageFile p2cFile;
        private StorageFile c2pFile;
        private StorageFileQueryResult query;
        private DateTimeOffset lastModification;
        public delegate void GestureCollectedHandler(Gesture gesture);
        private Dictionary<Page, Action<string>> pageEventDico;
        private BasicProperties prop;
        private string lastMessage;

        /// <summary>
        /// Gets an Instance of the classe if the it's already existing
        /// </summary>
        /// <value>LoggingHandler</value>
        public static GlobalMessageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalMessageManager();
                        }
                    }
                }
                return instance;
            }
        }

        // ctor
        private GlobalMessageManager()
        {
            pageEventDico = new Dictionary<Page, Action<string>>();
            GetQuery();
        }

        // Lance le file watching
        private async Task GetQuery()
        {
            StorageFolder folder = KnownFolders.PicturesLibrary;
            p2cFile = await folder.CreateFileAsync(Protocol.P2CFile, Windows.Storage.CreationCollisionOption.OpenIfExists);
            while (true)
            {
                try
                {
                    await ReadMessageAsync(null, null);
                    await Task.Delay(1000);
                }
                catch
                {
                }
            };
        }

        // Ouvre le fichier de communication p2c et lie son contenue
        private async Task ReadMessageAsync(Windows.Storage.Search.IStorageQueryResultBase sender, object args)
        {
            string text = await FileIO.ReadTextAsync(p2cFile);
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
                        pageEventDico[current].Invoke(text.Substring(0, index));
                    }
                }
            }
        }

        // Envoie un message au fichier de communication c2p
        public async Task SendMessageAsync(string message)
        {
            StorageFolder folder = KnownFolders.PicturesLibrary;
            //c2pFile = await folder.CreateFileAsync(Protocol.C2PFile, Windows.Storage.CreationCollisionOption.OpenIfExists);
            c2pFile = await folder.GetFileAsync(Protocol.C2PFile);
            await FileIO.WriteTextAsync(c2pFile, message + '-' + DateTime.Now.ToString());
        }

        private int GetTimeElapsed(DateTimeOffset current, DateTimeOffset last)
        {
            int currentMilli = current.Millisecond + (current.Second * 100) + (current.Minute * 60 * 100);
            int lastMilli = last.Millisecond + (last.Second * 100) + (last.Minute * 60 * 100);
            return currentMilli - lastMilli;
        }

        // Methode a utiliser pour s'enregistrer à l'évènement de message p2c
        public void RegisterToGlobalMessage(Page page, Action<string> action)
        {
            if (!pageEventDico.ContainsKey(page))
            {
                pageEventDico.Add(page, action);
            }
        }


    }
}