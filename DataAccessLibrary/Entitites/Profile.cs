using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DataAccessLibrary.Entitites
{
    public class Profile : IEntity
    {
        private string picture;


        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public long UserParentId { get; set; }
        [ManyToOne]
        public User UserParent { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }


        [JsonIgnore]
        public byte[] Face { get; set; }

        [JsonIgnore]
        public string FaceLink { get; set; }

        [JsonIgnore]
        public bool IsFaceLinked { get; set; }

        [JsonIgnore]
        [Ignore]
        public ImageSource PictureSource { get; set; }
  

        public string Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                SetImageAsync();
            }
        }
    
        private async void SetImageAsync()
        {
            if (Picture != null)
            {
                var bytes = Convert.FromBase64String(Picture);
                var buf = bytes.AsBuffer();
                var stream = buf.AsStream();
                var image = stream.AsRandomAccessStream();
                var decoder = await BitmapDecoder.CreateAsync(image);
                image.Seek(0);
                var output = new WriteableBitmap((int)decoder.PixelHeight, (int)decoder.PixelWidth);
                await output.SetSourceAsync(image);
                PictureSource = output;
            }
        }
    }
}
