﻿using System;
using System.Text.RegularExpressions;


namespace FlightPlanner.Core.Types
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


        public override int GetHashCode()
        {
            return _code.GetHashCode();
        }

        public override string ToString()
        {
            return _code;
        }


        private static void CheckIfValidCodeOrThrow(string input)
        {
            if (!Regex.IsMatch(input, "^[A-Z]{4}$"))
                throw new ArgumentException("Invalid ICAO code");
        }
    }
}
