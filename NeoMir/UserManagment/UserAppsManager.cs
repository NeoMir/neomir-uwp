using DataAccessLibrary;
using DataAccessLibrary.API;
using DataAccessLibrary.Entitites;
using NeoMir.Classes;
using NeoMir.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoMir.UserManagment
{
    public class UserAppsManager
    {
        #region PROPERTIES

        private static object syncRoot = new object();
        private static volatile UserAppsManager instance;

        #endregion

        #region CONSTRUCTOR

        // Obtient une instance de la classe si elle existe déjà
        public static UserAppsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new UserAppsManager();
                        }
                    }
                }
                return instance;
            }
        }

        private UserAppsManager()
        {
            UserManager.Instance.ProfileChanged += ProfileChanged;
        }

        #endregion

        #region METHODS

        private async void ProfileChanged()
        {
            await GetAppsForProfil();
        }

        // Recupère la liste des profil associé aux compte via l'API
        public async Task GetAppsForProfil()
        {
            List<UserApp> installed = DataAccess.GetEntities<UserApp>();
            DataAccess.DeleteTableEntries<UserApp>();
            foreach (var app in await APIManager.GetProfileApps(UserManager.Instance.CurrentProfile.Id))
            {
                DataAccess.AddEntity(app);
            }
        }

        public void Init()
        {
            //if (GlobalStatusManager.Instance.GlobalStatus == EGlobalStatus.FirstLaunch)
            //{
            GetDefaultApps();
            //
        }

        // Obtient les applications installées par défaut
        private void GetDefaultApps()
        {
            DataAccess.DeleteTableEntries<UserApp>();
            //DataAccess.AddEntity(new UserApp()
            //{
            //    AppId = 0,
            //    AppIconLink = "ms-appx:///Assets/AppsPage/maps.jpg",
            //    AppLink = "https://www.google.fr/maps/preview",
            //    ProfileId = 0,
            //    AppName = "Maps"
            //});

            //DataAccess.AddEntity(new UserApp()
            //{
            //    AppId = 1,
            //    AppIconLink = "ms-appx:///Assets/AppsPage/Gmail.png",
            //    AppLink = "https://mail.google.com",
            //    ProfileId = 0,
            //    AppName = "Gmail"
            //});

            //DataAccess.AddEntity(new UserApp()
            //{
            //    AppId = 2,
            //    AppIconLink = "ms-appx:///Assets/AppsPage/train.jpg",
            //    AppLink = "http://ec2-108-128-227-127.eu-west-1.compute.amazonaws.com",
            //    ProfileId = 0,
            //    AppName = "Trains"
            //});

            //DataAccess.AddEntity(new UserApp()
            //{
            //    AppId = 3,
            //    AppIconLink = "ms-appx:///Assets/AppsPage/EasyBrush.png",
            //    AppLink = "http://martinbaud.com/V1/EasyBrush.php",
            //    ProfileId = 0,
            //    AppName = "Brosse"
            //});

            //DataAccess.AddEntity(new UserApp()
            //{
            //    AppId = 4,
            //    AppIconLink = "ms-appx:///Assets/AppsPage/Facebook.png",
            //    AppLink = "http://facebook.com",
            //    ProfileId = 1,
            //    AppName = "FaceBook"
            //});

            //DataAccess.AddEntity(new UserApp()
            //{
            //    AppId = 5,
            //    AppIconLink = "ms-appx:///Assets/AppsPage/Epitech.jpg",
            //    AppLink = "https://intra.epitech.eu",
            //    ProfileId = 0,
            //    AppName = "Epitech"
            //});
        }

        #endregion
    }
}
