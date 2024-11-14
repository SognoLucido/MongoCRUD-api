using Logger.Model;
using Logger.Model.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;


namespace Logger
{
    public class LogService(ILogger<LogService> logger, IConfiguration _conf)
    {
        private readonly ILogger<LogService> log = logger;

        private readonly string? Logpath = _conf["Serilog:WriteTo:0:Args:configureLogger:WriteTo:0:Args:path"];




        ////plainText
        public async Task<string?> ReadLog(DateDtomodel rawdata,uint? seek)
        {

           

            bool filterRangeOff = rawdata.Enddate is null;
            DateTime[]? Datetimerange = new DateTime[2];
            string StartdateOnly = string.Empty;

            if (!filterRangeOff)
            {
                Datetimerange = ModeltoDateTime(rawdata);
            }
            else
            {
                StartdateOnly = rawdata.Startdate.ToString();
            }

             

            if (string.IsNullOrEmpty(Logpath)) return null;



            using FileStream fs = new FileStream(Logpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);


            if (seek is not null)
            {

                int linecount = 0;
                long offset = 1;
        

                for (; offset <= fs.Length && linecount <= seek; offset++)
                {
                    fs.Seek(-offset, SeekOrigin.End);


                    var bytevalue = fs.ReadByte();


                    if (bytevalue == 139)  // 139 128 226
                    {
                        var tempoffset = offset;

                        if (tempoffset + 1 > fs.Length) continue;
                        tempoffset++;
                        fs.Seek(-tempoffset, SeekOrigin.End);

                        bytevalue = fs.ReadByte();
                        if (bytevalue == 128)
                        {
                            if (tempoffset + 1 > fs.Length) continue;
                            tempoffset++;
                            fs.Seek(-tempoffset, SeekOrigin.End);


                            bytevalue = fs.ReadByte();
                            if (bytevalue == 226)
                            {
                                linecount++;
                            }
                        }
                    }
                }

                offset--;

            }
            using StreamReader reader = new StreamReader(fs);

            StringBuilder sb = new();

            bool msg = false;

            while (true)
            {


                string? line = await reader.ReadLineAsync();   
                if (!string.IsNullOrEmpty(line))
                {

                    string? checkValidline = line.Length >= 19 ? line.Substring(1, 19) : null;


                    if (checkValidline is not null)
                        if (DateTime.TryParseExact(checkValidline, "yyyy-MM-dd HH:mm:ss",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None, out var date))
                        {


                            if (filterRangeOff)
                            {
                                if (Datenormalizerv2(date, rawdata) == StartdateOnly) msg = true;
                                else msg = false;
                            }
                            else
                            {
                                if (Datetimerange[0] <= date && Datetimerange[1] >= date) msg = true;
                                else msg = false;
                            }



                        }



                 

                    if (msg)
                        sb.AppendLine(line);
                }
                else break;
            }


         

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



        private string Datenormalizerv2(DateTime fetched, DateDtomodel userinput)
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

        private DateTime[] ModeltoDateTime(DateDtomodel data)
        {


            DateTime[] StartNend =
             [
                 new(data.Startdate.Year, data.Startdate.Month ?? 0, data.Startdate.Day ?? 0, data.Startdate.Hour ?? 0, data.Startdate.Minute ?? 0, data.Startdate.Second ?? 0),
                new(data.Enddate!.Year, data.Enddate.Month , data.Enddate.Day , data.Enddate.Hour ?? 0, data.Enddate.Minute ?? 0, data.Enddate.Second ?? 0)

             ];

            return StartNend;
        }



    }
}
