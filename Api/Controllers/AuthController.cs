using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Commons.Utils;
using Repository.Interfaces;
using Repository.Models.Entities;
using Repository.Models.Payload.Requests;
using Repository.Models.Payload.Response;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<UserRole> _memberRepository;
    private readonly IMapper _mapper;

    public AuthController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _memberRepository = _unitOfWork.GetRequiredRepository<UserRole>();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        
        var user = await _memberRepository.FindByCondition(x =>
            x.Username == request.Username && x.Passphrase == request.Password).FirstOrDefaultAsync();

        if (user is null || user.Passphrase != request.Password)
        {
            return Unauthorized(Result<string>.Fail("Email or password is invalid!"));
        }

        var token = Jwt.CreateJwtToken(user, config["JwtSettings:Issuer"], config["JwtSettings:Issuer"],
            config["JwtSettings:Key"]);

        var result = new AuthenticationResult()
        {
            Token = token
        };
        
        return Ok(Result<AuthenticationResult>.Succeed(result));
    }
}

