using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoMir.Classes
{
    public class GlobalStatusManager
    {
        #region PROPERTIES

        private static object syncRoot = new object();
        private static volatile GlobalStatusManager instance;
        public EGlobalStatus GlobalStatus { get; set; }
        public enum EGlobalStatus
        {
            FirstLaunch,
            Paired,
        }

        #endregion

        #region CONSTRUCTOR

        // Obtient une instance de la classe si elle existe déjà
        public static GlobalStatusManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalStatusManager();
                        }
                    }
                }
                return instance;
            }
        }

        private GlobalStatusManager()
        {
            GlobalStatus = EGlobalStatus.Paired;
        }

        #endregion
    }



}
