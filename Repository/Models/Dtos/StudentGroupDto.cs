using Repository.Commons.Mapping;
using Repository.Models.Entities;

namespace Repository.Models.Dtos;

public class StudentGroupDto : IMapFrom<StudentGroup>
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? GroupName { get; set; }
}