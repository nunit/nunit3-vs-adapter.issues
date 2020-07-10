using System.Threading.Tasks;

namespace NSubstituteSkippedTestDemo
{
    public interface IThing
    {
        Task<string> GetNumberAsTextAsync(int number);
        string ToString();
    }
}