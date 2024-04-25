using AutoMapper;
using Repository.Commons.Mapping;
using Repository.Models.Entities;

namespace Repository.Models.Dtos;

public class StudentDto : IMapFrom<Student>
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? GroupId { get; set; }
    public StudentGroupDto Group { get; set; }

    public void Mapping(Profile profile)
    {
         profile.CreateMap<Student, StudentDto>()
             .ForMember(dest => dest.GroupId, 
                 opt => 
                     opt.MapFrom(src => src.Group!.Id));
    }
}