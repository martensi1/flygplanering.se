using Microsoft.AspNetCore.Http;
using System;

namespace FlightPlanner.Service.Extensions
{
    public static class SessionExtensions
    {
        public static void SetBool(this ISession session, string key, bool value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (IsValid(session))
            {
                session.Set(key, BitConverter.GetBytes(value));
            }
        }

        public static bool GetBool(this ISession session, string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (IsValid(session))
            {
                byte[] data = session.Get(key);

                if (data != null && data.Length == 1)
                {
                    return BitConverter.ToBoolean(data, 0);
                }
            }

            return false;
        }

        private static bool IsValid(ISession session)
        {
            return session != null;
        }
    }
}
