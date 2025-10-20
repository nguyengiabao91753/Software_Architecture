using Auth.API.Models;
using AuthAPI.Services.IServices;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecShare.Servicer.Auth;
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        this._jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(ApplicationUser user)
    {
        JwtSecurityTokenHandler tokenHandler = new(); // Tạo trình xử lý token
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret); // Chuyển đổi Secret thành byte array để dùng làm khóa bảo mật

        // Danh sách các claims (thông tin người dùng lưu trong token)
        var claimList = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email), // Email của người dùng
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), // ID của người dùng
            new Claim(JwtRegisteredClaimNames.Name, user.UserName.ToString()) // Tên người dùng
        };

        // Cấu hình thông tin cho token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer,
            Subject = new ClaimsIdentity(claimList), // Danh sách claims (danh sách role)
            Expires = DateTime.UtcNow.AddDays(30), // Thời gian hết hạn của token (30 ngày)
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), // Khóa bí mật để ký token
                SecurityAlgorithms.HmacSha256Signature // Thuật toán ký token
            )
        };

        // Tạo token dựa trên mô tả tokenDescriptor
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Chuyển token thành chuỗi JWT và trả về
        return tokenHandler.WriteToken(token);

    }
}
