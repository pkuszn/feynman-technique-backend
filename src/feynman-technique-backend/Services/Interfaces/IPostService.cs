namespace FeynmanTechniqueBackend.Services.Interfaces
{
    public interface IPostService<TCriteria, TResult>
        where TCriteria: ICriteria
        where TResult : class
    {
        Task<TResult> PostAsync(TCriteria c, CancellationToken cancellationToken);
    }
}
