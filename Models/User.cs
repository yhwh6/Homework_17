namespace Homework_17.Models
{
    public class User
    {
        public User(string FirstName, string LastName, string MiddleName, string PhoneNumber, string Email)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.MiddleName = MiddleName;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
        }
        public User() { }

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
