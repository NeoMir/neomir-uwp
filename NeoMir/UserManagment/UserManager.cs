using DataAccessLibrary;
using DataAccessLibrary.API;
using DataAccessLibrary.Entitites;
using NeoMir.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static NeoMir.Classes.GlobalStatusManager;

namespace NeoMir.UserManagment
{
    public class UserManager
    {
        #region PROPERTIES

        private static object syncRoot = new object();
        private static volatile UserManager instance;
        private Profile currentProfile;
        public event ProfileChangedHandler ProfileChanged;

        #endregion

        #region CONSTRUCTOR

        // Obtient une instance de la classe si elle existe déjà
        public static UserManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new UserManager();
                        }
                    }
                }
                return instance;
            }
        }

        private UserManager()
        {
            //TODO get list of user profile through the API
        }

        #endregion

        #region METHODS

        public List<Profile> Profiles
        {
            get { return DataAccess.GetEntities<Profile>(); }
        }

        public delegate void ProfileChangedHandler();

        public User CurrentUser { get; set; }

        // Profil actuel
        public Profile CurrentProfile
        {
            get { return currentProfile; }
            set
            {
                currentProfile = value;
                ProfileChanged?.Invoke();
            }
        }

        /// <summary>
        /// To call when a modification to the current profil properties has changed
        /// </summary>
        public void CurrentProfilUpdated()
        {
            DataAccess.UpdateEntity(UserManager.Instance.CurrentProfile);
            ProfileChanged?.Invoke();
        }

        public async Task Init()
        {
            if (GlobalStatusManager.Instance.GlobalStatus == EGlobalStatus.FirstLaunch)
            {
                GetDefaultProfiles();
            }
            else
            {
                Miror miror = DataAccess.GetMiror();
                List<Profile> list = await APIManager.GetUserProfiles(CurrentUser.Id);
                if (list.Count > 0)
                {
                    foreach(Profile profil in list)
                    {
                        if (!Profiles.Any(p => p.Id == profil.Id))
                        {
                            DataAccess.AddEntity(profil);
                        }
                    }

                    foreach (Profile profil in Profiles)
                    {
                        if (!list.Any(p => p.Id == profil.Id))
                        {
                            DeleteProfile(profil);
                        }
                        else
                        {
                            DataAccess.UpdateEntity(profil);
                        }
                    }
                }
            }
        }

        private async void DeleteProfile(Profile profile)
        {
            DataAccess.DeleteEntity(profile);
            if (profile.IsFaceLinked)
            {
                try
                {
                    StorageFolder folder = KnownFolders.PicturesLibrary;
                    string fileName = string.Format("Faces\\{0}.jpg", profile.Name);
                    StorageFile file = await folder.GetFileAsync(fileName);
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }

            }
        }

        private void GetDefaultProfiles()
        {
            DataAccess.AddEntity(new Profile()
            {
                Id = 0,
                Name = "Admin"
            });

            DataAccess.AddEntity(new Profile()
            {
                Id = 0,
                Name = "Guest"
            });

            /*DataAccess.AddEntity(new UserProfile()
            {
                Id = 3,
                Name = "Quentin"
            });

            DataAccess.AddEntity(new UserProfile()
            {
                Id = 4,
                Name = "Ambroise"
            });*/
        }

        #endregion
    }
}
