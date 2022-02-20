using FPSE.Core.Common;
using System;


namespace FPSE.Core.Types
{
    public sealed class IcaoCode
    {
        private string _code;


        public IcaoCode(string code)
        {
            CheckIfValidCodeOrThrow(code);
            _code = code;
        }

        public static implicit operator IcaoCode(string code)
        {
            if (code == null)
                return null;

            return new IcaoCode(code);
        }


        public override bool Equals(object obj)
        {
            return _code == (obj as IcaoCode)._code;
        }
        public bool Equals(IcaoCode p)
        {
            return this.Equals(p as object);
        }


        public static bool operator ==(IcaoCode left, IcaoCode right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(IcaoCode left, IcaoCode right)
        {
            return !left.Equals(right);
        }


        public override string ToString()
        {
            return _code;
        }
        public override int GetHashCode()
        {
            return _code.GetHashCode();
        }


        private static void CheckIfValidCodeOrThrow(string input)
        {
            if (!SimpleRegex.PatternExists(@"^[A-Z]{4}$", input))
                throw new ArgumentException("Invalid ICAO code");
        }
    }
}
