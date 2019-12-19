using System;

namespace BloggerAPI.DTO.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }

        public User(string firstName, string lastName, string email, string password, DateTime createdDate)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            CreatedDate = createdDate;
        }

        public User()
        {
            
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Email))
            {
                return $"{nameof(Id)}: {Id}, {nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}, {nameof(Email)}: {Email}, {nameof(Password)}: {Password}, {nameof(CreatedDate)}: {CreatedDate}, {nameof(LastModified)}: {LastModified}";

            }

            return "";
        }
    }
}
