using DataAccessLibrary.Entitites;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{

    public class Database
    {
        #region PROPERTIES

        private const string DbName = "NeoMirLocalDb.db";
        private static object syncRoot = new object();
        private static volatile Database instance;

        #endregion

        #region CONSTRUCTOR

        // Gets an Instance of the classe if the it's already existing
        public static Database Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Database();
                        }
                    }
                }
                return instance;
            }
        }

        private Database()
        {
            Db = new SQLiteConnection(DbName);
            Db.CreateTable<Miror>();
            Db.CreateTable<UserProfile>();
            Db.CreateTable<UserApp>();
        }

        #endregion

        #region METHODS

        // Gets the database instance
        public SQLiteConnection Db { get; }

        #endregion
    }
}
