using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace NeoMir.Classes.Communication
{
    public class GestureCollector
    {
        private static volatile GestureCollector instance;
        private static object syncRoot = new object();
        StorageFile file;
        public delegate void GestureCollectedHandler(string gesture);

        public event GestureCollectedHandler GestureCollected;

        /// <summary>
        /// Gets an Instance of the classe if the it's already existing
        /// </summary>
        /// <value>LoggingHandler</value>
        public static GestureCollector Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GestureCollector();
                        }
                    }
                }
                return instance;
            }
        }

        private GestureCollector()
        {
            GetQuery();
        }

        private async void GetQuery()
        {


            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".txt");
            var options = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            var query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(options);
            //subscribe on query's ContentsChanged event
            query.ContentsChanged += GetGesture;
            StorageFolder folder = KnownFolders.PicturesLibrary;
            file = await folder.GetFileAsync("gestures.txt");

            var files = await query.GetFilesAsync();
        }

        private async void GetGesture(Windows.Storage.Search.IStorageQueryResultBase sender, object args)
        {
            string text = await FileIO.ReadTextAsync(file);
            GestureCollected?.Invoke(text.Replace("\r\n", string.Empty));
        }
    }
}
