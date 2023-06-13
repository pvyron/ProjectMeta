using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.GrainInterfaces;
public interface IContactGrain : IGrainWithGuidKey
{
    ValueTask YourNameIs(string name);
    ValueTask<string> SayYourName();
}
