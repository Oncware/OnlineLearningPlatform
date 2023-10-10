using JustLearn1.Interface;
using JustLearn1.Repository.IRepository;
using System.Linq.Expressions;
using JustLearn1.Data;
using JustLearn1.Models;


namespace JustLearn1.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private JustDbContext _dbContex;
        public CategoryRepository(JustDbContext dbContex) : base(dbContex)
        {
            _dbContex = dbContex;
        }


        public void Update(Category obj)
        {
            _dbContex.Category.Update(obj);
        }



    }
}
