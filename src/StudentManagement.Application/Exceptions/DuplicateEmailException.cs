namespace StudentManagement.Application.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email) 
        : base($"A record with the email '{email}' already exists.")
    {
    }
}
