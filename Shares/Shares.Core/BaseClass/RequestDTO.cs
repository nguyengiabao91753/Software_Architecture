using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static Shares.Helper.Utils.SD;

namespace Shares.Core.BaseClass;
public class RequestDTO
{
    public ApiType ApiType { get; set; } = ApiType.GET;
    public string Url { get; set; }
    public object Data { get; set; }
    public string AccessToken { get; set; }

    public bool IsMultipart { get; set; } = false;
}
