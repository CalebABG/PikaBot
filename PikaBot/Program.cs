using System.Threading.Tasks;

namespace PikaBot
{
    public class Program
    {
        /* Use the async main (only available in C# 7.1 onwards). */
        public static async Task Main(string[] args)
        {
            var p = new PikaBot();
            await p.InitBot(args);
        }
    }
}
