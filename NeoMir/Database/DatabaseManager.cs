using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoMir.Database
{
    public class DatabaseManager
    {
        #region PROPERTIES

        private static object syncRoot = new object();
        private static volatile DatabaseManager instance;

        #endregion

        #region CONSTRUCTOR

        // Obtient une instance de la classe si elle existe déjà
        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new DatabaseManager();
                        }
                    }
                }
                return instance;
            }
        }

        private DatabaseManager()
        {

        }

        #endregion
    }
}
