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
                inputDate = inputDate.AddHours(8);
                Console.WriteLine(inputDate);


                DateTime univDate = inputDate.ToUniversalTime();
                univDate = univDate.AddMinutes(-60);

                long[] unixTimestamps = new long[13];
                for (int loop = 0; loop < 13; loop++)
                {
                    long unixTime = ((DateTimeOffset)univDate).ToUnixTimeSeconds();

                    unixTimestamps[loop] = unixTime;
                    Console.WriteLine(loop + 1 + ". " + univDate + " (UTC +0) " + unixTime);
                    Console.WriteLine();

                    univDate = univDate.AddMinutes(10);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
