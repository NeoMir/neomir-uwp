using Newtonsoft.Json;
using SQLite;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DataAccessLibrary.Entitites
{
    public class UserApp : IEntity
    {
        private string icon;

        [PrimaryKey]
        public long Id { get; set; }
      
        public bool IsLocalUsage { get; set; }

        public string Name { get; set; }
      
        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
               // SetImage();
            }
        }

        public string Description { get; set; }
    

        public string CompanyName { get; set; }
    

        [JsonIgnore]
        [Ignore]
        public ImageSource IconSource { get; set; }
    
        public long ProfileParentId { get; set; }
        public string Url { get; set; }
        public string ConfigUrl { get; set; }

        public async Task SetImage()
        {
            if (Icon != null)
            {
                try
                {
                    var bytes = Convert.FromBase64String(Icon);
                    var buf = bytes.AsBuffer();
                    var stream = buf.AsStream();
                    var image = stream.AsRandomAccessStream();
                    var decoder = await BitmapDecoder.CreateAsync(image);
                    image.Seek(0);
                    var output = new WriteableBitmap((int)decoder.PixelHeight, (int)decoder.PixelWidth);
                    await output.SetSourceAsync(image);
                    IconSource = output;

                }
                catch (Exception e)
                {

                }
              
            }
        }
    }
}
