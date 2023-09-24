namespace ShitPosterBot2.Shared;

public interface ISplashService
{
    public Task<string> GetRandomText();
}