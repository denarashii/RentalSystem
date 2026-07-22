namespace RentalSystem.Domain.Exceptions;

public class AccountDeactivatedException : Exception
{
    public AccountDeactivatedException()
        : base("This account has been deactivated. Please contact support.") { }
}