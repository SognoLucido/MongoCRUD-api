using Logger.Model;
using Logger.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;


namespace Logger
{
    public class LogService(ILogger<LogService> logger)
    {
        private readonly ILogger<LogService> log = logger;






        ////plainText
        public async Task<string> ReadLog(DateDtomodel rawdata,bool addtimeonly)
        {

            bool filterRangeOff = rawdata.Enddate is null;
            //DateTime[]? Datetimerange = filterRangeOff ?  null : new DateTime[2];
            DateTime[]? Datetimerange = new DateTime[2];
            string StartdateOnly  = string.Empty;

            if (!filterRangeOff) 
            {
                Datetimerange = ModeltoDateTime(rawdata,addtimeonly);
            }
            else
            {
                StartdateOnly = rawdata.Startdate.ToString();
            }

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

                    if (checkValidline is not null)
                        if (DateTime.TryParseExact(checkValidline, "yyyy-MM-dd HH:mm:ss",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None, out var date))
                        {


                            if (filterRangeOff)
                            {
                                if (datenormalizerv2(date, rawdata) == StartdateOnly) msg = true;
                                else msg = false;
                            }
                            else
                            {
                                if (Datetimerange[0] <= date && Datetimerange[1] >= date)  msg = true;
                                else msg = false;
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


        ///////////////////



        //private DateTime datenormalizer(DateTime fetched, DateTime userinput)
        //{

        //    var tempdata = new Datemap()
        //    {
        //       Month = fetched.Month,
        //       Day = fetched.Day,
        //       Hour = fetched.Hour,
        //       Minutes = fetched.Minute,
        //       Seconds = fetched.Second,
        //    };

        //    if (userinput.Second == 00) tempdata.Seconds = 00;
        //    if (userinput.Minute == 00) tempdata.Minutes = 00;
        //    if (userinput.Hour == 00) tempdata.Hour = 00;
        //    if (userinput.Day == 00) tempdata.Day = 00;
        //    if (userinput.Month == 00) tempdata.Month = 00;


        //    return new DateTime(fetched.Year,tempdata.Month,tempdata.Day,tempdata.Hour,tempdata.Minutes,tempdata.Seconds);
        //}
        private string datenormalizerv2(DateTime fetched, DateDtomodel userinput)
        {

            var tempdata = new Datemap()
            {
                Month = fetched.Month,
                Day = fetched.Day,
                Hour = fetched.Hour,
                Minutes = fetched.Minute,
                Seconds = fetched.Second,
            };

            if (userinput.Startdate.Second is null) tempdata.Seconds = 0;
            if (userinput.Startdate.Minute is null) tempdata.Minutes = 0;
            if (userinput.Startdate.Hour is null) tempdata.Hour = 0;
            if (userinput.Startdate.Day is null) tempdata.Day = 0;
            if (userinput.Startdate.Month is null) tempdata.Month = 0;

            //2024-11-03 16:43:21
            return $"{fetched.Year}-{tempdata.Month}-{tempdata.Day} {tempdata.Hour}:{tempdata.Minutes}:{tempdata.Seconds}";
        }

        private DateTime[] ModeltoDateTime(DateDtomodel data,bool adddatetime)
        {

            DateTime? temp;

            DateTime[] StartNend =
            [
                new(data.Startdate.Year, data.Startdate.Month ?? 0, data.Startdate.Day ?? 0, data.Startdate.Hour ?? 0, data.Startdate.Minute ?? 0, data.Startdate.Second ?? 0),
                new(data.Enddate!.Year, data.Enddate.Month , data.Enddate.Day , data.Enddate.Hour ?? 0, data.Enddate.Minute ?? 0, data.Enddate.Second ?? 0)

            ];





            return StartNend;
        }



    }
}
