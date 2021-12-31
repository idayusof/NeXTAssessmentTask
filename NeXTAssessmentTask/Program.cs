﻿using Newtonsoft.Json;
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
                Console.WriteLine("ENTER THE DATE AND TIME (eg: 23/07/2021 6:51pm) : ");
                DateTime inputDate = GetDate();
                DateTime currentDate = DateTime.Now;

                if (inputDate > currentDate)
                {
                    Console.WriteLine("INVALID DATE! PLEASE ENTER A CURRENT/PAST DATE : ");
                    inputDate = GetDate();
                    GetISSLocation(inputDate);
                } else
                {
                    GetISSLocation(inputDate);
                }

                inputDate = inputDate.AddHours(8);                

                //Console.WriteLine();
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

        static void GetISSLocation(DateTime date)
        {
            DateTime univDate = date.ToUniversalTime();
            univDate = univDate.AddMinutes(-60);

            Console.WriteLine("THE LOCATION OF ISS AT THIS TIME : ");
            Console.WriteLine();

            //long[] unixTimestamps = new long[13];
            for (int loop = 0; loop < 13; loop++)
            {
                long unixTime = ((DateTimeOffset)univDate).ToUnixTimeSeconds();

                //unixTimestamps[loop] = unixTime;

                Console.WriteLine(loop + 1 + ". " + univDate + " (UTC +0) " + unixTime);
                Console.WriteLine();

                univDate = univDate.AddMinutes(10);

                #region - calling API 
                HttpClient client = new HttpClient();
                var responseTask = client.GetAsync("https://api.wheretheiss.at/v1/satellites/25544/positions?timestamps=" + unixTime + "&units=miles");

                responseTask.Wait();

                if (responseTask.IsCompleted)
                {
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var messageTask = result.Content.ReadAsStringAsync();
                        messageTask.Wait();

                        List<Location> location = JsonConvert.DeserializeObject<List<Location>>(messageTask.Result);

                        foreach (var item in location)
                        {
                            #region - Get data from API based on latitude and longitude
                            var response = client.GetStringAsync("https://api.wheretheiss.at/v1/coordinates/" + item.latitude + "," + item.longitude);

                            var json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Result);
                            var formattedJson = Newtonsoft.Json.JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
                            Console.WriteLine(formattedJson);
                            Console.WriteLine();
                            #endregion
                        }
                    }
                }
                #endregion
            }
        }

        class Location
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }
    }
}
