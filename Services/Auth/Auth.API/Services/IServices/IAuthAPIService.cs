using AuthAPI.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Services.IServices
{
    public interface IAuthAPIService
    {
        Task<ResponseDto> Register(RegistrationRequestDto registrationRequestDto);

        Task<ResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}
