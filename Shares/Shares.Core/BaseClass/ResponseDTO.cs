using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Core.BaseClass
{
    public class ResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Result { get; set; }
        public string? Code { get; set; }
    }
}
