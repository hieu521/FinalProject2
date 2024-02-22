
using System.Text.Json.Serialization;

namespace BackEnd.Dtos.UserDtos;

public class AuthenticateResponse
{
    public int MemberId { get; set; }
    public string Title { get; set; }

    public bool Sex { get; set; }


    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
   /* public Role Role { get; set; }*/
    public DateTime Created { get; set; }
    public DateTime? Update { get; set; }
    public bool IsVerified { get; set; }
    public string? Avatar { get; set; }
    public string JwtToken { get; set; }
    [JsonIgnore]

    public string RefreshToken { get; set; }
}