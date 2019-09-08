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
        private const string DbName = "NeoMirLocalDb.db";
        private static object syncRoot = new object();
        private static volatile Database instance;


        /// <summary>
        /// Gets an Instance of the classe if the it's already existing
        /// </summary>
        /// <value>LoggingHandler</value>
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
        }

        /// <summary>
        /// Gets the database instance
        /// </summary>
        public SQLiteConnection Db { get; }
    }
}
