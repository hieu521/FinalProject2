using API.Data;
using API.DTOs;
using API.DTOs.Chats;
using API.Entities;
using API.Extensions;
using API.Services;
using AutoMapper.Execution;
using BackEnd.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Generators;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly StoreContext _context;
        private readonly SendMailBusinessService _emailService;

        public AccountController(UserManager<User> userManager, TokenService tokenService,
            StoreContext context , SendMailBusinessService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized();

            var userBasket = await RetrieveBasket(loginDto.Username);
            var anonBasket = await RetrieveBasket(Request.Cookies["buyerId"]);

            if (anonBasket != null)
            {
                if (userBasket != null) _context.Baskets.Remove(userBasket);
                anonBasket.BuyerId = user.UserName;
                Response.Cookies.Delete("buyerId");
                await _context.SaveChangesAsync();
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
                Basket = anonBasket != null ? anonBasket.MapBasketToDto() : userBasket?.MapBasketToDto()
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
        {
            var user = new User { UserName = registerDto.Username, Email = registerDto.Email };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            return StatusCode(201);
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userBasket = await RetrieveBasket(User.Identity.Name);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
                Basket = userBasket?.MapBasketToDto()
            };
        }

        [Authorize]
        [HttpGet("savedAddress")]
        public async Task<ActionResult<UserAddress>> GetSavedAddress()
        {
            return await _userManager.Users
                .Where(x => x.UserName == User.Identity.Name)
                .Select(user => user.Address)
                .FirstOrDefaultAsync();
        }


        [Authorize]
        [HttpGet("search-users/{term}")]
        public async Task<ActionResult<IEnumerable<UserInMessage>>> Search(string term)
        {
            var users = new List<User>();

            var usersInStaffRole = await _userManager.GetUsersInRoleAsync("Staff");

            // Filter users by the provided term (case-insensitive)
            users = usersInStaffRole
                .Where(user => user.UserName.ToLower().Contains(term.ToLower()))
                .ToList();


            var userInMessageList = new List<UserInMessage>();
            foreach (var userInfo in users)
            {
                var userName = userInfo.UserName.Split(" ");
                var userInMessage = new UserInMessage()
                {
                    Id = userInfo.Id,
                    Avatar = string.Empty,
                    FirstName = userName.Length == 2 ? userName[1] : "",
                    LastName = userName[0],
                    Email = userInfo.Email,
                };
                userInMessageList.Add(userInMessage);
            }

            return Ok(userInMessageList);
        }

        [Authorize]
        [HttpGet("load-staff")]
        public async Task<ActionResult<IEnumerable<UserInMessage>>> LoadStaff()
        {
            var users = new List<User>();

            var usersInStaffRole = await _userManager.GetUsersInRoleAsync("Staff");

            users = usersInStaffRole.ToList();

            var userInMessageList = new List<UserInMessage>();
            foreach (var userInfo in users)
            {
                var userName = userInfo.UserName.Split(" ");
                var userInMessage = new UserInMessage()
                {
                    Id = userInfo.Id,
                    Avatar = string.Empty,
                    FirstName = userName.Length == 2 ? userName[1] : "",
                    LastName = userName[0],
                    Email = userInfo.Email,
                };
                userInMessageList.Add(userInMessage);
            }

            return Ok(userInMessageList);
        }

        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId))
            {
                Response.Cookies.Delete("buyerId");
                return null;
            }

            return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(basket => basket.BuyerId == buyerId);
        }

        [Authorize]
        [HttpGet("get-all")]
        public async Task<IActionResult> Index()
        {
            var userList = _context.Users.ToList();

            foreach (var user in userList)
            {
                var roleTemp = await _userManager.GetRolesAsync(user);
                user.Role = string.Join(", ", roleTemp);
            }

            return Ok(userList.ToList());
        }

        [Authorize]
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateRoleDtos model)
        {
            var user = _context.Users.Find(id);

            var roleOld = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRoleAsync(user, roleOld.First());
            await _userManager.AddToRoleAsync(user, model.Role);

            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = _context.Users.Find(id);

            var chatUsersToDelete = _context.ChatUsers.Where(_ => _.UserId == id).ToList();
            _context.ChatUsers.RemoveRange(chatUsersToDelete);

            var messagesToDelete = _context.ChatMessages.Where(_ => _.FromUserId == id).ToList();
            _context.ChatMessages.RemoveRange(messagesToDelete);

            var chatIds = chatUsersToDelete.Select(_ => _.ChatId);
            var chatsToDelete = _context.Chats.Where(_ => chatIds.Contains(_.Id)).ToList();
            _context.Chats.RemoveRange(chatsToDelete);

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotpasswordDtos forgotpasswordDtos)
        {
            var user = await _userManager.FindByEmailAsync(forgotpasswordDtos.Email);
            if (user == null)
                return BadRequest("Invalid email address");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await SendMailForgotPassword(forgotpasswordDtos.Email, Request.Headers["origin"] ,token);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return NotFound();


            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("authenticate-google")]
        public async Task<ActionResult<UserDto>> AuthenticateGoogleLogin(GoogleLoginRequest model)
        {
            var response = await AuthenticateGoogleLogin(model, Request.Headers["origin"]);
            return Ok(response);
        }
        private async Task SendMailForgotPassword(string email, string origin, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var message = await System.IO.File.ReadAllTextAsync("Emails/User/forgotpassword.html");
            message = message.Replace("[[Name]]", user.UserName);
            var verifyUrl = $"{origin}/resetpassword?token={token}&email={email}";


            message = message.Replace("[[Link]]", verifyUrl);

            await _emailService.SendEmailAsync(user.Email, "Reset Password", message);
        }

        private async Task<UserDto> AuthenticateGoogleLogin(GoogleLoginRequest model, string origin)
        {
            var googleUser = ValMemberIdateGoogleToken(model.TokenMemberId);

            var existingUser = await _userManager.FindByEmailAsync(googleUser.Email);

            if (existingUser == null)
            {
                existingUser = CreateUserFromGoogleData(googleUser);

                await _userManager.CreateAsync(existingUser, "Pa$$w0rd");
                await _userManager.AddToRoleAsync(existingUser, "Member");
            }
            else
            {
                if (!existingUser.IsInternal)
                {
                    existingUser = UpdateUserFromGoogleData(existingUser, googleUser);
                    await _userManager.UpdateAsync(existingUser);

                }
                else
                {
                    //await sendAlreadyRegisteredEmail(existingUser.Email, origin);
                    return new UserDto();

                }

            }

            if (existingUser == null || !await _userManager.CheckPasswordAsync(existingUser, "Pa$$w0rd"))
                return new UserDto();

            var userBasket = await RetrieveBasket(existingUser.UserName);
            var anonBasket = await RetrieveBasket(Request.Cookies["buyerId"]);

            if (anonBasket != null)
            {
                if (userBasket != null) _context.Baskets.Remove(userBasket);
                anonBasket.BuyerId = existingUser.UserName;
                Response.Cookies.Delete("buyerId");
                await _context.SaveChangesAsync();
            }

            return new UserDto
            {
                Id = existingUser.Id,
                Email = existingUser.Email,
                Token = await _tokenService.GenerateToken(existingUser),
                Basket = anonBasket != null ? anonBasket.MapBasketToDto() : userBasket?.MapBasketToDto()
            };

        }
        private GoogleUser ValMemberIdateGoogleToken(string googleToken)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestUri = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={googleToken}&client_id=1092302126288-kke2oa6bd7ml02r01ol2sb52eu672ef9.apps.googleusercontent.com";

                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;

                    var googleUser = JsonConvert.DeserializeObject<GoogleUser>(responseContent);
                    return googleUser;
                }

                return null;
            }
        }
        private User CreateUserFromGoogleData(GoogleUser googleUser)
        {
            var newUser = new User
            {
                UserName = googleUser.Email,
                Email = googleUser.Email,
                IsInternal = false,
            };
            return newUser;
        }
        private User UpdateUserFromGoogleData(User existingUser, GoogleUser googleUser)
        {
            existingUser.Email = googleUser.Email;
    
            return existingUser;
        }


    }
}