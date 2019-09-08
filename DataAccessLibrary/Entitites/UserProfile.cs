using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Entitites
{
    public class UserProfile : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Face { get; set; }
        public string FaceLink { get; set; }
    }
}
