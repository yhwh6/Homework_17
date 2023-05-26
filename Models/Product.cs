namespace Homework_17.Models
{
    public class Product
    {
        public Product(string Email, string Code, string ProductName)
        {
            this.Email = Email;
            this.Code = Code;
            this.ProductName = ProductName;
        }
        public Product() { }

        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Code { get; set; }
        public string? ProductName { get; set; }
    }
}