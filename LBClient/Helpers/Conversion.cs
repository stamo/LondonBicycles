using System;

namespace LondonBicycles.Client.Helpers
{
    public static class Conversion
    {
        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
        {
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0);
            dtDateTime = dtDateTime.AddMilliseconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }
    }
}
