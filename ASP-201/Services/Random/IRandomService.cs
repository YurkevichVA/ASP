namespace ASP_201.Services.Random
{
    public interface IRandomService
    {
        string ConfirmCode(int length);
        string RandomString(int length);
        string RandomFileName();
    }
}
