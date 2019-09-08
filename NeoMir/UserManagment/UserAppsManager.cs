using DataAccessLibrary;
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
        private static object syncRoot = new object();
        private static volatile UserAppsManager instance;

        /// <summary>
        /// Gets an Instance of the classe if the it's already existing
        /// </summary>
        /// <value>LoggingHandler</value>
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
        }

        public void Init()
        {
            if (GlobalStatusManager.Instance.GlobalStatus == EGlobalStatus.FirstLaunch)
            {
                GetDefaultApps();
            }
        }

       /// <summary>
       /// Gets apps that are installled by default
       /// </summary>
       /// <returns></returns>
        private void GetDefaultApps()
        {
            DataAccess.DeleteTableEntries<UserApp>();
            DataAccess.AddEntity(new UserApp()
            {
                AppId = 0,
                AppIconLink = "ms-appx:///Assets/AppsPage/YoutubeIcon.png",
                AppLink = "https://www.youtube.com/",
                ProfileId = 0,
                AppName = "Youtube"
            });

            DataAccess.AddEntity(new UserApp()
            {
                AppId = 1,
                AppIconLink = "ms-appx:///Assets/AppsPage/mapsIcon.png",
                AppLink = "https://www.google.fr/maps/preview",
                ProfileId = 0,
                AppName = "Maps"
            });

            DataAccess.AddEntity(new UserApp()
            {
                AppId = 2,
                AppIconLink = "ms-appx:///Assets/AppsPage/gmailIcon.png",
                AppLink = "https://mail.google.com",
                ProfileId = 0,
                AppName = "Gmail"
            });

            DataAccess.AddEntity(new UserApp()
            {
                AppId = 3,
                AppIconLink = "ms-appx:///Assets/AppsPage/trainIcon.png",
                AppLink = "http://ec2-108-128-227-127.eu-west-1.compute.amazonaws.com",
                ProfileId = 0,
                AppName = "Trains"
            });

            DataAccess.AddEntity(new UserApp()
            {
                AppId = 4,
                AppIconLink = "ms-appx:///Assets/AppsPage/facebookIcon.png",
                AppLink = "http://facebook.com",
                ProfileId = 1,
                AppName = "FaceBook"
            });

            DataAccess.AddEntity(new UserApp()
            {
                AppId = 5,
                AppIconLink = "ms-appx:///Assets/AppsPage/epitechIcon.png",
                AppLink = "https://intra.epitech.eu",
                ProfileId = 2,
                AppName = "Epitech"
            });
        }
    }
}
