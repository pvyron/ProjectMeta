namespace ProMe.Abstractions;
public interface IEmailService
{
    ValueTask<Guid> SendVerificationEmail(string email);
}
