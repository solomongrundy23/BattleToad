﻿using System;
using System.Collections.Generic;
using System.Text;
using BattleToad.Extensions;

namespace BattleToad.Users
{
    class Users
    {
        private static Users Instance;
        public static Users GetInstance()
        {
            lock (Instance)
            {
                if (Instance == null) Instance = new Users();
                return Instance;
            }
        }
        public List<User> List = new List<User>();
    }

    class User
    {
        private static byte[] salt = new byte[]
            {
                        87,
                        23,
                        6,
                        111,
                        23,
                        211,
                        223,
                        215,
                        23,
                        188,
                        91,
                        26,
                        74
                    };
        public User(string name, string password, Users users = null)
        {
            PasswordChecker.CheckNewPassword(password);
            Name = name;
            Password = password;
            if (users != null) users.List.Add(this);
        }
        public string Name;
        private string _Password;
        public string Password
        {
            get => _Password;
            set => _Password = Hash.GetHash(Encoding.Unicode.GetBytes(value + salt), Hash.Type.SHA256);
        }
        public bool CheckPassword(string password)
            => Hash.GetHash(Encoding.Unicode.GetBytes(password), Hash.Type.SHA256) == _Password;
        public bool CheckPasswordHash(string password_hash) => _Password == password_hash;
    }

    internal static class PasswordChecker
    {
        public static void CheckNewPassword(string password)
        {
            if (password == null) throw new Exception("Пароль не может быть null");
            if (password.Length < 8) throw new Exception("Пароль менее 8 символов");
            if (!CheckChars(password)) throw new Exception("Пароль должен содержать большие и маленькие буквы и цифры");
        }

        private static bool CheckChars(string password) => ContainsDigit(password) && ContainsLowerLetter(password) && ContainsUpperLetter(password);

        private static bool ContainsLowerLetter(string password)
        {
            foreach (char c in password)
            {
                if ((Char.IsLetter(c)) && (Char.IsLower(c)))
                    return true;
            }
            return false;
        }

        private static bool ContainsUpperLetter(string password)
        {
            foreach (char c in password)
            {
                if ((Char.IsLetter(c)) && (Char.IsUpper(c)))
                    return true;
            }
            return false;
        }

        private static bool ContainsDigit(string password)
        {
            foreach (char c in password)
            {
                if (Char.IsDigit(c))
                    return true;
            }
            return false;
        }

        private static bool ContainsSimbol(string password)
        {
            foreach (char c in password)
            {
                if (Char.IsSymbol(c) || Char.IsPunctuation(c))
                    return true;
            }
            return false;
        }
    }
}
