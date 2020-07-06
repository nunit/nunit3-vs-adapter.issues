using System.Threading.Tasks;

namespace NSubstituteSkippedTestDemo
{
    public class Thing : IThing
    {
        public async Task<string> GetNumberAsTextAsync(int number)
        {
            await Task.Delay(1);
            return number.ToString();
        }
    }
}