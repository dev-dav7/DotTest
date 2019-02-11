using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotTest.VK
{
    enum WallTypeForRequest
    {
        User,
        Public,
        Undefined,//Такого id не сущетсвует
        Vague//id может быть как user так и public
    }
}
