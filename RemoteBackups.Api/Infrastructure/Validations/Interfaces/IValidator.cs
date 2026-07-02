namespace RemoteBackups.Api.Infrastructure.Validations.Interfaces
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T instance);
    }
}
