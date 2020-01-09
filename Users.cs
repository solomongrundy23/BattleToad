using System.Collections.Generic;
using System.Text;
using BattleToad.Ext;

namespace BattleToad.Users
{
    class Users
    {
        public List<User> List = new List<User>();
    }

    class User
    {
        public User(string name, string password, Users users = null)
        {
            Name = name;
            Password = password;
            if (users != null) users.List.Add(this);
        }
        public string Name;
        private string _Password;
        public string Password
        {
            get => _Password;
            set => _Password = Hash.GetHash(Encoding.Unicode.GetBytes(value), Hash.Type.SHA256);
        }
        public bool CheckPassword(string password)
            => Hash.GetHash(Encoding.Unicode.GetBytes(password), Hash.Type.SHA256) == _Password;
    }
}
