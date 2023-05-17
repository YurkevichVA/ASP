namespace ASP_201.Data.Entity
{
    public class User
    {
        public Guid      Id                 { get; set; }
        public string    Login              { get; set; } = null!;
        public string    Email              { get; set; } = null!;
        public string    RealName           { get; set; } = null!;
        public string    PasswordHash       { get; set; } = null!;
        public string    PasswordSalt       { get; set; } = null!;
        public string?   Avatar             { get; set; }

        public DateTime  RegisterDt         { get; set; } // момент реєстрації
        public DateTime? LastEnter          { get; set; } // останній вхід до сайту
        public string?   EmailCode          { get; set; }

        //// Додано 2023-04-19 робота з Profile
        public bool     IsEmailPublic       { get; set; } = false;
        public bool     IsRealNamePublic    { get; set; } = false;
        public bool     IsDateTimesPublic   { get; set; } = false;
    }
}
