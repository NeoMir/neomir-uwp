using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoMir.Classes
{
    public class GlobalStatusManager
    {
        private static object syncRoot = new object();
        private static volatile GlobalStatusManager instance;
     
        /// <summary>
        /// Gets an Instance of the classe if the it's already existing
        /// </summary>
        /// <value>LoggingHandler</value>
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

        public EGlobalStatus GlobalStatus { get; set; }

        private GlobalStatusManager()
        {
            GlobalStatus = EGlobalStatus.Paired;
        }
    }


    public enum EGlobalStatus
    {
        FirstLaunch,
        Paired,
    }
}
