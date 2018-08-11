using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapingTool
{
    class Program
    {
        static void Main(string[] args)
        {
            bool applicationContinue = true;
            string input = string.Join(" ", args);

            while (applicationContinue)
            {
                //Select input from either arguments or input
                input = Console.ReadLine();

                //Validate Input




            }

            Console.WriteLine($"End Of Execution: {DateTime.Now}");
        }
    }
}
