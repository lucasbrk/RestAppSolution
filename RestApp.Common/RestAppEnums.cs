using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestApp.Common
{
    public static class ApEnums
    {
        public enum InputClass
        {
            none = 0,
            mini = 100,
            small = 101,
            medium = 102,
            large = 103,
            xlarge = 104,            
            xxlarge = 105
        }

        public enum ValidationTypeEnum
        {
            error = 100,
            warning = 101
        }
    }
}
