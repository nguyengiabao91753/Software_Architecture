using Shares.Core.BaseClass;
using Shares.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Base.Auth
{
    public interface IAuthAPIService
    {
        Task<ResponseDTO> Register(RegistrationRequestDto registrationRequestDto);

        Task<ResponseDTO> Login(LoginRequestDto loginRequestDto);
    }
}
