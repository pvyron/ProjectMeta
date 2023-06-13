using ProMe.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Grains;
internal class ContactGrain : Grain, IContactGrain
{
    string? _name;

    public async ValueTask<string> SayYourName()
    {
        if (_name is null)
            return "I'm not yet named";
        
        return $"I'm {_name} and my Id is {this.GetPrimaryKey()}";
    }

    public async ValueTask YourNameIs(string name)
    {
        _name = name;
    }
}
