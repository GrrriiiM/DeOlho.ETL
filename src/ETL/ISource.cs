using System.Threading.Tasks;

namespace DeOlho.ETL
{
    public interface ISource<T>
    {
        Task<T> Execute();
    }
}