namespace ASP_201.Services.Validation
{
    public interface IValidationService
    {
        bool Validate(string source, params ValidationTerms[] terms);
    }
}
