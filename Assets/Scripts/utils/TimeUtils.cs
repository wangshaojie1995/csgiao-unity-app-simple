using UnityEngine;

namespace utils
{
    public class TimeUtils
    {
        public static long GetMsStamp()
        {
            return (long)(Time.time * 1000.0f);
        }
    }
}