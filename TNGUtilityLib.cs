using TNG.Shared.Lib.Intefaces;
using System.Data;

namespace TNG.Shared.Lib
{

    public class TNGUtilityLib : ITNGUtiltityLib
    {
        private IAuthenticationService _auth;

        public TNGUtilityLib(IAuthenticationService auth)
        {
            this._auth = auth;
        }
        /// <summary>
        /// Convert datatime from database
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DateTime? convertDateTimeFromDB(object data)
        {
            return Convert.IsDBNull(data) ? (DateTime?)null : Convert.ToDateTime(data);
        }

        /// <summary>
        /// convert utc time to corresponding timezone
        /// </summary>
        /// <param name="utcTime"></param>
        /// <param name="storeTime"></param>
        /// <returns></returns>
        public object convertDateTimeFromUtc(object utcTime)
        {
            try
            {
                string storeTimeZone = this._auth.User.TimeZone;
                var zone = storeTimeZone.Split('|');
                TimeZoneInfo timeZone;
                try
                {
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(zone[0]);
                }
                catch (TimeZoneNotFoundException)
                {
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(zone[1]);
                }
                DateTime returnTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(utcTime), timeZone);
                return returnTime;
            }
            catch
            {
                return utcTime;
            }
        }

        public DateTime getCurrentTime(DateTime? dateTime = null)
        {
            dateTime = dateTime == null ? DateTime.UtcNow : dateTime;
            DateTime returnValue = (DateTime)convertDateTimeFromUtc(dateTime);
            return returnValue;
        }

        public object ConvertTimeToUtc(object dateTime)
        {
            try
            {
                string storeTimeZone = this._auth.User.TimeZone;
                var zone = storeTimeZone.Split('|');
                TimeZoneInfo timeZone;
                try
                {
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(zone[0]);
                }
                catch (TimeZoneNotFoundException)
                {
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(zone[1]);
                }
                var returnTime = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(dateTime), timeZone);
                return returnTime;
            }
            catch
            {
                return dateTime;
            }
        }

        public string getRandomString()
        {
            int length = 5;
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var returnValue = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return returnValue;
        }

        /// <summary>
        /// CONCATENATE
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <returns></returns>
        public string CONCATENATE(string FirstName, string LastName)
        {
            string fullName = String.Concat(FirstName, " ", LastName);
            return fullName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public string ConvertTimeFormat(DateTime d1)
        {

            TimeSpan timespan = d1.TimeOfDay;
            DateTime time = DateTime.Today.Add(timespan);
            string displayTime = time.ToString("hh:mm tt");

            return displayTime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>

        public string ConvertDateFormat(DateTime d1)
        {

            string actualDate = d1.ToString("MMMM dd, yyyy",System.Globalization.CultureInfo.InvariantCulture);

            return actualDate;
        }

          /// <summary>
        ///  Get Duration between two dates
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>

        public string GetDuration(DateTime startTime, DateTime endTime)
        {
            TimeSpan timeDiff = endTime - startTime;
            string result = string.Empty;
            if (timeDiff < TimeSpan.FromMinutes(60))
            {
                result = String.Format("{0} mins", timeDiff.Minutes);

            }
            else
            {
                result = String.Format("{0} hrs", timeDiff.Hours);
            }

            return result;

        }


        


    }
}