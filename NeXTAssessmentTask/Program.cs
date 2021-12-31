using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NeXTAssessmentTask
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("ENTER THE DATE AND TIME (eg: 23/07/2021 6:51pm) : ");
                DateTime inputDate = GetDate();
                DateTime currentDate = DateTime.Now;

                if (inputDate > currentDate)
                {
                    Console.Write("INVALID DATE! PLEASE ENTER A CURRENT/PAST DATE : ");
                    inputDate = GetDate();
                    GetTimestamps(inputDate);
                } else
                {
                    GetTimestamps(inputDate);
                }

                inputDate = inputDate.AddHours(8);                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static DateTime GetDate()
        {
            string date = Console.ReadLine();
            Console.WriteLine();

            DateTime inputDate = DateTime.Parse(date);
            inputDate = inputDate.AddHours(8);

            return inputDate;
        }

        static void GetTimestamps(DateTime date)
        {
            String urlTimestamps = "";

            DateTime univDate = date.ToUniversalTime();
            univDate = univDate.AddMinutes(-60);

            Console.WriteLine("Retrieving location... ");
            Console.WriteLine();

            DateTime[] dateList = new DateTime[13];
            long[] unixTimestamps = new long[13];

            for (int loop = 0; loop < 13; loop++)
            {
                dateList[loop] = univDate; 
                long unixTime = ((DateTimeOffset)univDate).ToUnixTimeSeconds();

                unixTimestamps[loop] = unixTime;
                                                
                urlTimestamps = loop == 0 ? unixTimestamps[loop].ToString() : urlTimestamps + "," + unixTimestamps[loop].ToString();

                univDate = univDate.AddMinutes(10);                
            }

            GetISSLocation(urlTimestamps, dateList);
        }

        static void GetISSLocation( String url, DateTime[] date)
        {
            #region - calling API 
            HttpClient client = new HttpClient();
            var responseTask = client.GetAsync("https://api.wheretheiss.at/v1/satellites/25544/positions?timestamps=" + url + "&units=miles");

            responseTask.Wait();
            #endregion

            if (responseTask.IsCompleted)
            {
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var messageTask = result.Content.ReadAsStringAsync();
                    messageTask.Wait();

                    List<Location> location = JsonConvert.DeserializeObject<List<Location>>(messageTask.Result);

                    int loop = 0;
                    foreach (var item in location)
                    {
                        #region - Get data from API based on latitude and longitude
                        var response = client.GetStringAsync("https://api.wheretheiss.at/v1/coordinates/" + item.latitude + "," + item.longitude);

                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Result);
                        var formattedJson = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);

                        Console.WriteLine(loop + 1 + ". " + date[loop] + " (UTC +0) ");
                        Console.WriteLine();
                        Console.WriteLine(formattedJson);
                        Console.WriteLine();

                        loop++;
                        #endregion
                    }
                }
            }
        }

        class Location
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }
    }
}
