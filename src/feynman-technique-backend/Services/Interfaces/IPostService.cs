namespace FeynmanTechniqueBackend.Services.Interfaces
{
    public interface IPostService<TCriteria, TResult>
        where TCriteria: ICriteria
    {
        Task<TResult> PostAsync(TCriteria criteria, CancellationToken cancellationToken);
    }

    //TODO: Interfejsy
}


