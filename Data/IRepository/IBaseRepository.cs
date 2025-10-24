namespace Data.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> FindByIdAsync(long id);
        Task<int> Add(T request);
        Task<int> Update(T request);
    }
}
