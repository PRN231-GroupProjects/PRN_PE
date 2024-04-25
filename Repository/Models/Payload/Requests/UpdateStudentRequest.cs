namespace Repository.Models.Payload.Requests;

public class UpdateStudentRequest
{
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? DateOfBirth { get; set; }
    public int GroupId { get; set; }
}