using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelToy.Environment
{
    enum AxisDirections
    {
        XPositive = 1,
        XNegative = 2,
        YPositive = 4,
        YNegative = 8,
        ZPositive = 16,
        ZNegative = 32,

        All       = 63
    }
}
