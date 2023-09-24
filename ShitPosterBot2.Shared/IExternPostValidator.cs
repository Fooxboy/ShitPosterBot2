using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Shared;

public interface IExternPostValidator
{
    public Task<bool> IsValid(Post post);
}