using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Utils
{

    const double _mmToFeet = 0.0032808399;

    public static double mmToFeet(double mmValue)
    {
        return mmValue * _mmToFeet;
    }


}

