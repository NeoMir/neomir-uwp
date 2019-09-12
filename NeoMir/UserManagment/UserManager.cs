using DataAccessLibrary;
using DataAccessLibrary.API;
using DataAccessLibrary.Entitites;
using NeoMir.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoMir.UserManagment
{
    public class UserManager
    {
        private static object syncRoot = new object();
        private static volatile UserManager instance;
        private UserProfile currentProfile;


        /// <summary>
        /// Gets an Instance of the classe if the it's already existing
        /// </summary>
        /// <value>LoggingHandler</value>
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

        public List<UserProfile> Profiles
        {
            get { return DataAccess.GetEntities<UserProfile>(); }
        }

        public delegate void ProfileChangedHandler();

        public event ProfileChangedHandler ProfileChanged;

        /// <summary>
        /// Current profile 
        /// </summary>
        public UserProfile CurrentProfile
        {
            get { return currentProfile; }
            set
            {
                currentProfile = value;
                ProfileChanged?.Invoke();
            }
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
                DataAccess.DeleteTableEntries<UserProfile>();
                for (int i = 0; i < list.Count; i++)
                {
                    DataAccess.AddEntity(new UserProfile() { Id = i, Name = list[i] });
                }
            }
        }

        private void GetDefaultProfiles()
        {
            DataAccess.AddEntity(new UserProfile()
            {
                Id = 1,
                Name = "Marwin"
            });

            DataAccess.AddEntity(new UserProfile()
            {
                Id = 2,
                Name = "Robin"
            });

            DataAccess.AddEntity(new UserProfile()
            {
                Id = 3,
                Name = "Quentin"
            });

            DataAccess.AddEntity(new UserProfile()
            {
                Id = 4,
                Name = "Ambroise"
            });
        }
    }
}
