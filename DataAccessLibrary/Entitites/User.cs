using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Entitites
{
    [Table("User")]
    public class User : IEntity
    {
        public long Id { get; set; }

        public string FirstName { get; set; }
      
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsLogged { get; set; }
    }
}
