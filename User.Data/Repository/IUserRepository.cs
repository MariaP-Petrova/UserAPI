namespace User.Data.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<Entity.User>> GetAllAsync();
        Task <Entity.User?> GetByIdAsync(int id);
        Task <Entity.User> AddAsync(Entity.User user);
        Task UpdateAsync (Entity.User user);
        Task DeleteAsync(int id);
        Task<User.Data.Entity.User> GetByUsernameAsync(string username);


    }
}
