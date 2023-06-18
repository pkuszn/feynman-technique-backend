namespace FeynmanTechniqueBackend.Services.Interfaces
{
    public interface IGetService<TResponse>
    {
        public Task<TResponse> GetAsync(CancellationToken cancellationToken);
    }

    public interface IGetService<TCriteria, TResponse>
        where TCriteria: ICriteria
        where TResponse: class
    {
        public Task<TResponse> GetAsync(TCriteria criteria, CancellationToken cancellationToken);
    }
}
