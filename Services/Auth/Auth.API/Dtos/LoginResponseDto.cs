using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.Dtos
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }

}
