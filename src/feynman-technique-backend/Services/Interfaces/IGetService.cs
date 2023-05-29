namespace FeynmanTechniqueBackend.Services.Interfaces
{
    public interface IGetService<TResponse>
    {
        public Task<TResponse> GetAsync();
    }
}
