namespace Repository.Models.Payload.Requests;

public class GetAllStudentsPaginatedRequest
{
    public int? GroupId { get; set; }
    public string? MinBirthYear { get; set; }
    public string? MaxBirthYear { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}