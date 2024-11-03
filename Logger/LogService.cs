using Logger.Model;
using Microsoft.Extensions.Logging;
using System.Text;


namespace Logger
{
    public class LogService(ILogger<LogService> logger)
    {
        private readonly ILogger<LogService> log = logger;






        ////plainText
        public async Task<string> ReadLog(DateTime start /*, DateTime? end*/)
        {
            var test = Directory.GetCurrentDirectory();

            var ok = Path.Combine(test, "Logs/log.txt");

            var testx = Path.Exists(ok);

            using FileStream fileStream = new FileStream(ok, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new StreamReader(fileStream);

            StringBuilder sb = new();

            bool msg = false;

            while (true)
            {
                string? line = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {

                    string? checkValidline = line.Length >= 19 ? line.Substring(0, 19) : null;

                    if(checkValidline is not null)
                    if (DateTime.TryParse(checkValidline, out var date))
                    {  
                        // check if the date corrispond to range of the one selected on the endpoint
                        if (datenormalizer(date,start) == start)
                        {
                            msg = true;
                        }
                        else
                        {
                            msg = false;
                        }


                    }



                    // Console.WriteLine();

                    if (msg)
                        sb.AppendLine(line);
                }
                else break;
            }


            //  Console.WriteLine(sb.ToString());

            return sb.ToString();



        }


        private DateTime datenormalizer(DateTime fetched, DateTime userinput)
        {

            var tempdata = new DateMap()
            {
               Month = fetched.Month,
               Day = fetched.Day,
               Hour = fetched.Hour,
               Minutes = fetched.Minute,
               Seconds = fetched.Second,
            };

            if (userinput.Second == 00) tempdata.Seconds = 00;
            if (userinput.Minute == 00) tempdata.Minutes = 00;
            if (userinput.Hour == 00) tempdata.Hour = 00;
            if (userinput.Day == 00) tempdata.Day = 00;
            if (userinput.Month == 00) tempdata.Month = 00;


            return new DateTime(fetched.Year,tempdata.Month,tempdata.Day,tempdata.Hour,tempdata.Minutes,tempdata.Seconds);
        }


        //plainText
        public async Task WriteLog(string? body, LogLevelError lev, bool throwerror)
        {
            body ??= "msg-null";

            if (throwerror)
                try
                {
                    throw new NotImplementedException();
                }
                catch (Exception ex)
                {

                    switch (lev)
                    {
                        case LogLevelError.Verbose: log.LogTrace(ex, body); break;
                        case LogLevelError.Debug: log.LogDebug(ex, body); break;
                        case LogLevelError.Information: log.LogInformation(ex, body); break;
                        case LogLevelError.Warning: log.LogWarning(ex, body); break;
                        case LogLevelError.Error: log.LogError(ex, body); break;
                        case LogLevelError.Critical: log.LogCritical(ex, body); break;

                    }

                }
            else
            {
                switch (lev)
                {
                    case LogLevelError.Verbose: log.LogTrace(body); break;
                    case LogLevelError.Debug: log.LogDebug(body); break;
                    case LogLevelError.Information: log.LogInformation(body); break;
                    case LogLevelError.Warning: log.LogWarning(body); break;
                    case LogLevelError.Error: log.LogError(body); break;
                    case LogLevelError.Critical: log.LogCritical(body); break;

                }
            }




        }





    }
}
