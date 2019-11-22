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
    public class GlobalMessageCollector
    {
        private static volatile GlobalMessageCollector instance;
        private static object syncRoot = new object();
        private StorageFile file;
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
        public static GlobalMessageCollector Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalMessageCollector();
                        }
                    }
                }
                return instance;
            }
        }

        private GlobalMessageCollector()
        {
            pageEventDico = new Dictionary<Page, Action<string>>();
            GetQuery();
        }

        private async Task GetQuery()
        {
            StorageFolder folder = KnownFolders.PicturesLibrary;
            file = await folder.GetFileAsync(Protocol.ComFile);
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
                        pageEventDico[current].Invoke(text.Substring(0, index));
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

        public void RegisterToGlobalMessage(Page page, Action<string> action)
        {
            if (!pageEventDico.ContainsKey(page))
            {
                pageEventDico.Add(page, action);
            }
        }
    }
}