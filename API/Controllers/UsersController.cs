using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {            
            return Ok(await _userRepository.GetMembersAsync());
        }


        // api/users/3      
        [HttpGet("{userName}")]
        public async Task<ActionResult<MemberDto>> GetUser(string userName)
        {
            return await _userRepository.GetMemberAsync(userName);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(userName);

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Erro ao atualizar o usuario");
        }
    }
}