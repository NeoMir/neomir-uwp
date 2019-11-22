using DataAccessLibrary;
using DataAccessLibrary.API;
using DataAccessLibrary.Entitites;
using NeoMir.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        if (Profiles.Where(p => p.Name == list[i]).FirstOrDefault() == null)
                        {
                            DataAccess.AddEntity(new UserProfile() { Id = i + 1, Name = list[i], IsFaceLinked= false });
                        }
                    }
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

            //DataAccess.AddEntity(new UserProfile()
            //{
            //    Id = 2,
            //    Name = "Robin"
            //});

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
