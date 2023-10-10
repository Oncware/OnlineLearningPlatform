using JustLearn1.Models;
using JustLearn1.Models.Interfaces;

namespace JustLearn1.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        Product? GetProductDetail(int id);
        void Save();

    }
}
