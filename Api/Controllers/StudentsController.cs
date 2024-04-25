using System.Net.Mail;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Commons.Extensions;
using Repository.Interfaces;
using Repository.Models;
using Repository.Models.Dtos;
using Repository.Models.Entities;
using Repository.Models.Payload.Requests;
using Repository.Models.Payload.Response;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<StudentGroup> _studentGroupRepository;

    public StudentsController(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _studentRepository = _unitOfWork.GetRequiredRepository<Student>();
        _studentGroupRepository = _unitOfWork.GetRequiredRepository<StudentGroup>();
    }

    [Authorize(Policy = "StaffOnly")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result<StudentDto>>> GetStudentById(int id)
    {
        var student = await _studentRepository.FindByCondition(x => x.Id == id).Include(x => x.Group).FirstOrDefaultAsync();
        if (student is null)
        {
            return NotFound(Result<string>.Fail("Student does not exist"));
        }

        var result = _mapper.Map<StudentDto>(student);
        return Ok(Result<StudentDto>.Succeed(result));
    }

    [Authorize(Policy = "StaffOnly")]
    [HttpGet]
    public async Task<ActionResult<Result<PaginatedList<StudentDto>>>> GetAlLStudentPaginated([FromQuery] GetAllStudentsPaginatedRequest request)
    {
        var students = _studentRepository.GetAll().Include(x => x.Group).AsQueryable();

        var pageNumber = request.PageIndex ?? 1;
        var pageSize = request.PageSize ?? 10;

        if (request.GroupId is not null)
        {
            students = students.Where(x => x.GroupId == request.GroupId);
        }

        if (request.MinBirthYear is not null && request.MaxBirthYear is not null)
        {
            var minBirthYear = request.MinBirthYear.ToDateTime();
            var maxBirthYear = request.MaxBirthYear.ToDateTime();

            if (minBirthYear == DateTime.MinValue || maxBirthYear == DateTime.MaxValue)
            {
                return BadRequest(Result<string>.Fail("Wrong DateTime format"));
            }
            
            students = students.Where(x => x.DateOfBirth <= maxBirthYear && x.DateOfBirth >= minBirthYear);
        }

        var list = await students.Paginate(pageNumber, pageSize).ToListAsync();
        var result =  _mapper.Map<List<StudentDto>>(list);

        return Ok(Result<PaginatedList<StudentDto>>.Succeed(new PaginatedList<StudentDto>(result, result.Count, pageNumber, pageSize)));
    }

    [Authorize(Policy = "StaffOnly")]
    [HttpPost]
    public async Task<ActionResult<Result<StudentDto>>> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var studentGroup = await _studentGroupRepository.GetByIdAsync(request.GroupId);

        if (studentGroup is null)
        {
            return NotFound(Result<string>.Fail("Pet group does not exist"));
        }

        if (request.Email is null || !request.Email.IsMailAddress())
        {
            return BadRequest(Result<string>.Fail("Invalid email format"));
        }

        if (request.FullName is null || request.FullName.Length < 18 || request.FullName.Length > 100)
        {
            return BadRequest(Result<string>.Fail("Invalid name length"));
        }

        if (!request.FullName.IsNameFormat())
        {
            return BadRequest(Result<string>.Fail("Invalid name format"));
        }

        if (request.DateOfBirth is null)
        {
            return BadRequest(Result<string>.Fail("Date of birth cannot be null"));
        }
        var dob = request.DateOfBirth.ToDateTime();
        if (dob == DateTime.MinValue)
        {
            return BadRequest(Result<string>.Fail("Wrong Datetime format"));
        }


        var student = new Student()
        {
            Email = request.Email,
            GroupId = studentGroup.Id,
            DateOfBirth = dob,
            FullName = request.FullName
        };

        var result = await _studentRepository.AddAsync(student);
        await _unitOfWork.CommitAsync();

        return Ok(Result<StudentDto>.Succeed(_mapper.Map<StudentDto>(result)));
    }

    [Authorize(Policy = "StaffOnly")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Result<StudentDto>>> UpdateStudent([FromRoute]int id, [FromBody]UpdateStudentRequest request)
    {
        var studentGroup = await _studentGroupRepository.GetByIdAsync(request.GroupId);
        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
        {
            return NotFound(Result<string>.Fail("Student does not exist"));
        }
        
        if (studentGroup is null)
        {
            return NotFound(Result<string>.Fail("Pet group does not exist"));
        }

        if (request.Email is null || !request.Email.IsMailAddress())
        {
            return BadRequest(Result<string>.Fail("Invalid email format"));
        }

        if (request.FullName is null || request.FullName.Length < 18 || request.FullName.Length > 100)
        {
            return BadRequest(Result<string>.Fail("Invalid name length"));
        }

        if (!request.FullName.IsNameFormat())
        {
            return BadRequest(Result<string>.Fail("Invalid name format"));
        }

        if (request.DateOfBirth is null)
        {
            return BadRequest(Result<string>.Fail("Date of birth cannot be null"));
        }
        var dob = request.DateOfBirth.ToDateTime();
        if (dob == DateTime.MinValue)
        {
            return BadRequest(Result<string>.Fail("Wrong Datetime format"));
        }

        student.FullName = request.FullName;
        student.Email = request.Email;
        student.DateOfBirth = dob;
        student.GroupId = request.GroupId;

        var result = _studentRepository.Update(student);
        await _unitOfWork.CommitAsync();

        return Ok(Result<StudentDto>.Succeed(_mapper.Map<StudentDto>(result)));
    }

    [Authorize(Policy = "StaffOnly")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Result<StudentDto>>> DeleteStudent(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
        {
            return NotFound(Result<string>.Fail("Student does not exist"));
        }

        var result = _studentRepository.Remove(id);
        await _unitOfWork.CommitAsync();

        return Ok(Result<StudentDto>.Succeed(_mapper.Map<StudentDto>(result)));
    }
}