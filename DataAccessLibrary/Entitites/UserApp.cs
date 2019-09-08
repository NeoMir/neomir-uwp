﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Entitites
{
    public class UserApp : IEntity
    {
        [PrimaryKey]
        public int AppId { get; set; }
      
        public int ProfileId { get; set; }

        public string AppName { get; set; }

        public byte[] AppIcon { get; set; }

        public string AppIconLink { get; set; }

        public string AppLink { get; set; }

        public bool IsLocalUsage { get; set; }

    }
}
