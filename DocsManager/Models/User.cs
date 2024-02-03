namespace DocsManager.Models;

public class User
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string PersonalId { get; set; }
    public string FreelanceWorkId { get; set; }
    public string BankNumber { get; set; }
    public string BankName { get; set; }
}