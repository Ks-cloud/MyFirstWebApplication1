using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MyFirstWebApplication1.Contracts;
using MyFirstWebApplication1.Domain;
using MyFirstWebApplication1.Controllers.v1.Requests;
using MyFirstWebApplication1.Controllers.v1.Responses;
using MyFirstWebApplication1.Services;

namespace MyFirstWebApplication1.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetALL()
        {
            return Ok(await _postService.GetPostsAsync());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {
            var post = _postService.GetPostByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody]CreatePostRequest postRequest)
        {
            var post = new Post {Name = postRequest.Name};

            await _postService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse {Id = post.Id};
            return Created(locationUri, response);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody]UpdatePostRequest updatePostRequest)
        {
            var post = new Post
            {
                Id = postId,
                Name = updatePostRequest.Name
            };

            var updatedPost = await _postService.UpdatePostAsync(post);

            if (updatedPost)
            {
                return Ok(updatedPost);
            }

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var deletedPost = await _postService.DeletePostAsync(postId);

            if (deletedPost)
            {
                return NoContent();
            }

            return NotFound();

        }
    }
}
