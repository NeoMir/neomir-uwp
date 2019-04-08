using CSNamedPipe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tester
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NamedPipeServer PServer1;
        StorageFile file;

        public MainPage()
        {
            this.InitializeComponent();
            //PServer1 = new NamedPipeServer(@"\\.\pipe\myNamedPipe1", 0);
            //PServer1.Start();

            GetQuery();
        }

        private async void GetQuery()
        {
        

            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".txt");
            var options = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
            var query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(options);
            //subscribe on query's ContentsChanged event
            query.ContentsChanged += Query_ContentsChangedAsync;
            StorageFolder folder = KnownFolders.PicturesLibrary;
            file = await folder.GetFileAsync("gestures.txt");

            var files =  await query.GetFilesAsync();
        }

        private async Task GetGesture()
        {
            
            string text = await Windows.Storage.FileIO.ReadTextAsync(file);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                GestureText.Text = text;
            });

            //var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            //ulong size = stream.Size;
            //using (var inputStream = stream.GetInputStreamAt(0))
            //{
            //    using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
            //    {
            //        uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
            //        string text = dataReader.ReadString(numBytesLoaded);
            //        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
            //            GestureText.Text = text;
            //        });
                    
            //    }
            //}
            
        }

        private async void Query_ContentsChangedAsync(Windows.Storage.Search.IStorageQueryResultBase sender, object args)
        {
           await GetGesture();
        }
    
        public void WaitForGesture()
        {
           
        }

    }
}
