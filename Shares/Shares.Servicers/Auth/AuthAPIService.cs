using Microsoft.AspNetCore.Identity;
using Orders.Infrastructure.Data;
using Shares.Base.Auth;
using Shares.Core.Auth;
using Shares.Core.BaseClass;
using Shares.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Servicer.Auth
{
    public class AuthAPIService : IAuthAPIService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthAPIService(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<ResponseDTO> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.Username.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            var loginresponse = new LoginResponseDto();

            if (user == null || !isValid)
            {
                loginresponse.User = null;
                loginresponse.Token = null;
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Username or password is incorrect",
                    Code = "-1",
                    Result = loginresponse
                };



            }
            //Lấy ra các roles của user đang login
            var roles = await _userManager.GetRolesAsync(user);
            //Truyền thông tin user và roles vừa lấy để sinh Token
            var token = _jwtTokenGenerator.GenerateToken(user);
            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber
            };
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token
            };
            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Login Successful",
                Code = "0",
                Result = loginResponseDto
            };

        }

        public async Task<ResponseDTO> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.UserName,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded && user.Id != null)
                {
                    // Tạo cặp khóa RSA
                    //(user.PublicKey, user.RsaPrivateKeyEncrypted) = RsaKeyPairHelper.GenerateKeyPair(user.PasswordHash);
                    await _userManager.UpdateAsync(user);
                    return new ResponseDTO
                    {
                        IsSuccess = true,
                        Message = "User Registration Successful",
                        Code = "0",
                        Result = user.Id
                    };
                }
                else
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = string.Join(", ", result.Errors.FirstOrDefault().Description),
                        Code = "-1",
                        Result = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Code = "-1",
                    Result = null
                };
            }
        }
    }
}
