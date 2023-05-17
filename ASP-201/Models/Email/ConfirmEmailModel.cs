namespace ASP_201.Models.Email
{
    public class ConfirmEmailModel
    {
        public string RealName    { get; set; } = null!;
        public string Email       { get; set; } = null!;
        public string EmailCode   { get; set; } = null!;
        public string ConfirmLink { get; set; } = null!;
    }
}
