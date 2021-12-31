using System;

namespace NeXTAssessmentTask
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("ENTER THE DATE AND TIME (eg: 23/07/2021 6:51pm) : ");
                string date = Console.ReadLine();
                Console.WriteLine();

                DateTime inputDate = DateTime.Parse(date);
                Console.WriteLine(inputDate);

                long unixTime = ((DateTimeOffset)inputDate).ToUnixTimeSeconds();

                Console.WriteLine("UNIXTIMESTAMP : " + unixTime);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
