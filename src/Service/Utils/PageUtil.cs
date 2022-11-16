using System;

namespace FlightPlanner.Service
{
    public static class PageUtil
    {
        public static string GenerateRandomElementId()
        {
            var guid = Guid.NewGuid();
            guid = EnsureStartsWithLetter(guid);

            return guid.ToString();
        }

        private static Guid EnsureStartsWithLetter(Guid guid)
        {
            var bytes = guid.ToByteArray();

            if ((bytes[3] & 0xf0) < 0xa0)
            {
                bytes[3] |= 0xc0;
                return new System.Guid(bytes);
            }
            return guid;
        }
    }
}
