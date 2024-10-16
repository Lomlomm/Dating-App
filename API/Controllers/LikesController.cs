using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;
    
    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username) 
    {
        var sourceUserId = User.GetUserId();
        var likedUser = await _userRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _likesRepository.GetUserWithLikesAsync(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("No le puedes dar Like a tu usuario");

        var userLike = await _likesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

        if (userLike != null) return BadRequest("Ya le has dado Like a este usuario");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Ha ocurrido un error");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams lp)
    {
        lp.UserId = User.GetUserId();

        var users = await _likesRepository.GetUserLikesAsync(lp);

        Response.AddPaginationHeader(
            new PaginationHeader(
                users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages
            ));

        return Ok(users);
    }
}
