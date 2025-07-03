using ProOb_RPG.GameInput;
using ProOb_RPG.GameModel;
using ProOb_RPG.GameOutput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProOb_RPG.GameModel;

class Program
{
    static void Main()
    {
        ModelGameSystem model = ModelGameSystem.GetInstance();
        InputGameSystem controls = new();
        OutputGameSystem view = new(controls);
        ModelGameSystem.GetInstance().Run();
    }
}

