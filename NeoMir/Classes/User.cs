using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoMir.Classes
{
    /// <summary>
    /// User class for storing user information
    /// </summary>
    class User
    {
        // PROPERTIES
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        // CONSTRUCTOR
        public User(string firstname, string lastname)
        {
            FirstName = firstname;
            LastName = lastname;
        }

        // METHODS
    }
}
