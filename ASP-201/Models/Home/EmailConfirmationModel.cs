namespace ASP_201.Models.Home
{
    public class EmailConfirmationModel
    {
        public string Host     { get; set; } = null!;
        public int    Port     { get; set; }
        public string Email    { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool   Ssl      { get; set; }
    }
}
