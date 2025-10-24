namespace Data.IRepository
{
        public interface IGenericRepository<T>
        {
            Task<List<T>> GetAll();
            Task<T> GetById(int id);
            Task<int> AddUpdate(T entity);
            Task<int> Delete(int id);
        }
    
}
