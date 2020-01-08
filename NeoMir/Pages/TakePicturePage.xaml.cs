using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using NeoMir.Classes;
using Windows.Graphics.Imaging;
using Windows.Media.MediaProperties;
using Windows.Storage.FileProperties;
using NeoMir.UserManagment;
using NeoMir.Classes.Com;
using DataAccessLibrary;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace NeoMir.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class TakePicturePage : Page
    {
        private MediaCapture mediaCapture;
        private DeviceInformationCollection cameraDevices;
        private DeviceInformation backFacingDevice;
        private DeviceInformation preferredDevice;

        public TakePicturePage()
        {
            this.InitializeComponent();
            FrameManager.NavigatedEvent += NavigateOn;
            GlobalMessageManager.Instance.RegisterToGlobalMessage(this, PythonResponse);
            InitDevices();
        }


        #region Capture

        private async void InitDevices()
        {
            cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            backFacingDevice = cameraDevices.FirstOrDefault(c => c.EnclosureLocation?.Panel == Windows.Devices.Enumeration.Panel.Back);

            // but if that doesn't exist, take the first camera device available
            //preferredDevice = cameraDevices[1];
        }

        private async Task InitializeCameraAsync()
        {
            // Create MediaCapture
            mediaCapture = new MediaCapture();

            // Initialize MediaCapture and settings
            await mediaCapture.InitializeAsync(
                new MediaCaptureInitializationSettings
                {
                    VideoDeviceId = preferredDevice.Id
                });
            await StartCapture();
            await CountDownCapture();
        }

        private async Task StartCapture()
        {
            // Set the preview source for the CaptureElement
            Capture.Source = mediaCapture;

            // Start viewing through the CaptureElement 
            await mediaCapture.StartPreviewAsync();
        }

        private async Task StopCapture()
        {
            if (mediaCapture != null && mediaCapture.CameraStreamState == Windows.Media.Devices.CameraStreamState.Streaming)
            {
                await mediaCapture.StopPreviewAsync();
                // Create MediaCapture
                mediaCapture = new MediaCapture();

                // Initialize MediaCapture and settings
                await mediaCapture.InitializeAsync(
                    new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = preferredDevice.Id
                    });
            }
        }

        private async Task RestCameraAsync()
        {
            if (mediaCapture != null && mediaCapture.CameraStreamState == Windows.Media.Devices.CameraStreamState.Streaming)
            {
                await mediaCapture.StopPreviewAsync();
            }
            mediaCapture = null;
        }

        private async Task CountDownCapture()
        {
            CountDownGrid.Visibility = Visibility.Visible;
            for (int i = 5; i >= 0; i--)
            {
                CountDown.Text = i.ToString();
                await Task.Delay(1000);
            }
            CountDownGrid.Visibility = Visibility.Collapsed;
            Button_Click(null, null);
        }

        /// <summary>
        /// Display the proccesing picture string when the photo has been taken
        /// </summary>
        private async void PhotoRegistred()
        {
            LineGrid.Visibility = Visibility.Collapsed;
            PhotoTookGrid.Visibility = Visibility.Visible;
            await GlobalMessageManager.Instance.SendMessageAsync(Protocol.RequiredFaceSave);
            PhotoTookMessage.Text = Globals.GlobalStrings.PhotoTookProcessing;
        }

        /// <summary>
        /// Pyhton response to the registred picture
        /// </summary>
        /// <returns>task</returns>
        private async void PythonResponse(string str)
        {
            if (str == Protocol.RegisterFaceOK)
            {
                LineGrid.Visibility = Visibility.Collapsed;
                PhotoTookGrid.Visibility = Visibility.Visible;
                PhotoTookMessage.Text = Globals.GlobalStrings.PhotoRegistred;
                await Task.Delay(4000);
                UserManager.Instance.CurrentProfile.IsFaceLinked = true;
                UserManager.Instance.CurrentProfilUpdated();
                FrameManager.GoTo(FrameManager.MainPageFrame);
                LineGrid.Visibility = Visibility.Visible;
                PhotoTookGrid.Visibility = Visibility.Collapsed;
                await GlobalMessageManager.Instance.SendMessageAsync(Protocol.StartGesture);
            }
            else if (str == Protocol.RegisterFaceKO)
            {
                LineGrid.Visibility = Visibility.Collapsed;
                PhotoTookGrid.Visibility = Visibility.Visible;
                PhotoTookMessage.Text = Globals.GlobalStrings.PhotoNotRegistred;
                await Task.Delay(4000);
                LineGrid.Visibility = Visibility.Visible;
                PhotoTookGrid.Visibility = Visibility.Collapsed;
                await InitializeCameraAsync();
            }
            else if (str == Protocol.StopCamOK)
            {
                await InitializeCameraAsync();
            }
        }
        #endregion

        #region Event

        /// <summary>
        /// Central button clicked to take a picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await StopCapture();
            var myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
            StorageFile file = await myPictures.SaveFolder.CreateFileAsync(
                Path.Combine(Protocol.NewFaceFolder, UserManager.Instance.CurrentProfile.Name + ".jpg"),
                CreationCollisionOption.ReplaceExisting);

            using (var captureStream = new InMemoryRandomAccessStream())
            {
                await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);

                using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var decoder = await BitmapDecoder.CreateAsync(captureStream);
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

                    var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
        };
                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                    await encoder.FlushAsync();
                }
            }
            PhotoRegistred();
        }

        // Go to Home Page
        private void HomeButton_Tapped(object sender, RoutedEventArgs e)
        {
            FrameManager.GoTo(FrameManager.MainPageFrame);
        }

        // Evenement de navigation
        private async void NavigateOn(Page page)
        {
            if (page == this)
            {
                await GlobalMessageManager.Instance.SendMessageAsync(Protocol.StopCam);
            }
            else
            {
                await RestCameraAsync();
                PhotoTook = null;
            }
        }

        #endregion
    }
}
