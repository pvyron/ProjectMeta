namespace ProMe.GrainInterfaces;
public interface IContactGrain : IGrainWithGuidKey
{
    ValueTask YourNameIs(string name);
    ValueTask<string> SayYourName();
}
