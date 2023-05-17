using System;
using System.Text;

namespace ASP_201.Services.Transliteration
{
    public class TransliterationServiceV1 : ITransliterationService
    {
        private readonly string _ukrainianSimple =  "АБВГҐДЕЗИІКЛМНОПРСТУФабвгґдезиклмнопрстуф";
        private readonly string _englishSimple =    "ABVHGDEZYIKLMNOPRSTUFabvhgdezyklmnoprstuf";
        public string Transliterate(string source)
        {
            string result = source;

            result = result.Replace("зг", "zgh").Replace("Зг", "Zgh");
            result = result.Replace("ь", "").Replace("Ь", "").Replace("\'", "");

            for(int i = 0; i < _ukrainianSimple.Length; i++)
            {
                result = result.Replace(_ukrainianSimple[i], _englishSimple[i]);
            }

            return result
                .Replace("Ш", "Sh").Replace("ш", "sh")
                .Replace("Х", "Kh").Replace("х", "kh")
                .Replace("Ц", "Ts").Replace("ц", "ts")
                .Replace("Ч", "Ch").Replace("ч", "Ch")
                .Replace("Щ", "Shch").Replace("щ", "Shch")
                .Replace(" Ї", " Yi").Replace(" ї", " yi")
                .Replace("Ї", "I").Replace("ї", "i")
                .Replace(" Й", " Y").Replace(" й", " i")
                .Replace("Й", "Y").Replace("й", "i")
                .Replace(" Є", " Ye").Replace(" є", " ye")
                .Replace("Є", "Ie").Replace("є", "ie")
                .Replace(" Ю", " Yu").Replace(" ю", " yu")
                .Replace("Ю", "Iu").Replace("ю", "iu")
                .Replace(" Я", " Ya").Replace(" я", " ya")
                .Replace("Я", "Ia").Replace("я", "ia")
                .Replace(' ', '-').Replace("+", "plus")
                .Replace('?', '-');
        }
    }
}
