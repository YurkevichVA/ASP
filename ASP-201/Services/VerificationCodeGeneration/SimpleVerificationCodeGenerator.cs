namespace ASP_201.Services.VerificationCodeGeneration
{
    public class SimpleVerificationCodeGenerator : IVerificationCodeGenerator
    {
        private readonly string _symbols = "1234567890qwertyuiopasdfghjklzxcvbnm";
        private readonly System.Random _random = new();
        public string GenerateVerificationCode()
        {

            string code = "";

            for (int i = 0; i < 6; i++)
            {
                code += _symbols[_random.Next(_symbols.Length)];
            }

            return code;
        }
    }
}
