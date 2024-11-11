
using Logger;
using Logger.Model.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mongocrud.Logger.Integration.test.Models;
using SharpCompress.Common;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Mongocrud.Logger.Integration.test
{
    public class FileUtils(IConfiguration _conf) 
    {
        private readonly string Logpath  = _conf["Serilog:WriteTo:0:Args:configureLogger:WriteTo:0:Args:path"]!;


        /// <summary>
        /// Returns an array of booleans corresponding to whether each argument is found in the file.
        /// Do not insert duplicates in the Findmatch(parameters[]).
        /// </summary>
        /// <returns></returns>
        public async Task<bool[]> Findmatch(object[] arg)
        {
            bool[] matches = new bool[arg.Length];

            var ToSerilogLognaames = new List<string>();

            foreach (var error in arg) 
            {
                switch (error) 
                {
                    case LogLevelError.Critical:ToSerilogLognaames.Add("Fatal"); break;
                    case "Exception": ToSerilogLognaames.Add("Exception");break;
                    default: ToSerilogLognaames.Add(error.ToString()); break; 
                }
            }


            using FileStream fileStream = new FileStream(Logpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new(fileStream);

            string? line = "";

            do
            {
                line = await reader.ReadLineAsync() ?? "";
              

                foreach ( var item in ToSerilogLognaames)
                {
                    Console.WriteLine();

                    if (line.Contains(item))
                    {
                        matches[ToSerilogLognaames.IndexOf(item)] = true;
                        continue;
                    }


                }

            }
            while (!string.IsNullOrEmpty(line));


            return matches;


        }



        public async Task<DateDtomodel> RangeCraftResposte(string data)
        {

            string[] split = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length <= 2) throw new Exception("unable to test range , 3 or more logs needed");

            Console.WriteLine();


            for(int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Substring(0,19);
            }

            Console.WriteLine();

           split = [.. split.Order()];


            DateTime[] daterange = new DateTime[2]; //  0 min , 1 max
           


            for (int i = split.Length-1 , y = daterange.Length-1; i >= 0 && y >= 0; i--)
            {

                if (DateTime.TryParseExact(split[i], "yyyy-MM-dd HH:mm:ss",
                                         System.Globalization.CultureInfo.InvariantCulture,
                                         System.Globalization.DateTimeStyles.None, out daterange[y]))
                {

                    if(y == 0 && daterange[0] == daterange[1]) continue;

                    y--;
                }


            }

            if (daterange[0] >= daterange[1]) throw new Exception("failed to extract a valid range from existing data body");



            return new DateDtomodel()
            {
                Startdate = new()
                {
                    Year = daterange[0].Year,
                    Month = daterange[0].Month,
                    Day = daterange[0].Day,
                    Hour = daterange[0].Hour,
                    Minute = daterange[0].Minute,
                    Second = daterange[0].Second
                },
                Enddate = new()
                {
                    Year = daterange[1].Year,
                    Month = daterange[1].Month,
                    Day = daterange[1].Day,
                    Hour = daterange[1].Hour,
                    Minute = daterange[1].Minute,
                    Second = daterange[1].Second
                }

            };


        }






    }
}
