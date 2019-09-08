using System;
using System.Collections.Generic;
using DataAccessLibrary.Entitites;

namespace DataAccessLibrary
{

    public static class DataAccess
    {
        public static bool AddEntity<EntityType>(EntityType entity) 
            where EntityType : IEntity
        {
            try
            {
                Database.Instance.Db.Insert(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public static bool UpdateEntity<EntityType>(EntityType entity)
            where EntityType : IEntity, new()
        {
            try
            {
                Database.Instance.Db.Update(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public static List<EntityType> GetEntities<EntityType>()
            where EntityType : IEntity, new()
        {
            return Database.Instance.Db.Table<EntityType>().ToList();
        }

        public static Miror GetMiror()
        {
            List<Miror> query = Database.Instance.Db.Table<Miror>().ToList();
            if (query.Count > 0)
            {
                return query[0];
            }
            return null;
        }
    }
}
