using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.Identity.Contracts.Requests;
using Minesweeper.Identity.Data.Entities;

namespace Minesweeper.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AspNetUserManager<AppUser> _userManager;

        public UsersController(AspNetUserManager<AppUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            // TODO: Validation
            var user = new AppUser
            {
                DisplayName = request.DisplayName,
                Email = request.Email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                UserName = request.Email,
                TwoFactorEnabled = false
            };

            var creationResult = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
            if (creationResult.Succeeded)
            {
                return Ok();
            }

            // TODO: Improve the mapping (replace string.empty, etc)
            foreach (var err in creationResult.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return BadRequest(ModelState);
        }
    }
}