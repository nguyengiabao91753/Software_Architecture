using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Application.Dtos.BaseClass;
public class ResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public object? Result { get; set; }
    public string? Code { get; set; }
}
