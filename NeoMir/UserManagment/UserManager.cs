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
        private UserProfile currentProfile;
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

        public List<UserProfile> Profiles
        {
            get { return DataAccess.GetEntities<UserProfile>(); }
        }

        public delegate void ProfileChangedHandler();

        // Profil actuel
        public UserProfile CurrentProfile
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
                List<string> list = await APIManager.GetUserProfiles(DataAccess.GetMiror().Usermail);
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        UserProfile profile = Profiles.Where(p => p.Name == list[i]).FirstOrDefault();
                        if (profile == null)
                        {
                            DataAccess.AddEntity(new UserProfile() { Id = i + 1, Name = list[i], IsFaceLinked = false });
                        }
                        else
                        {
                            profile.Id = i + 1;
                        }
                    }
                    List<UserProfile> profiles = Profiles;
                    foreach (UserProfile profil in Profiles)
                    {
                        if (list.Where(p => p == profil.Name).FirstOrDefault() == null)
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

        private async void DeleteProfile(UserProfile profile)
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
            DataAccess.AddEntity(new UserProfile()
            {
                Id = 1,
                Name = "Admin"
            });

            DataAccess.AddEntity(new UserProfile()
            {
                Id = 2,
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
