using Microsoft.CodeAnalysis.Text;
using System;

namespace RoslynEditor.Core.BraceMatching
{
    public struct BraceMatchingResult : IEquatable<BraceMatchingResult>
    {
        public TextSpan LeftSpan { get; }
        public TextSpan RightSpan { get; }

        public BraceMatchingResult(TextSpan leftSpan, TextSpan rightSpan)
            : this()
        {
            LeftSpan = leftSpan;
            RightSpan = rightSpan;
        }

        public bool Equals(BraceMatchingResult other) => LeftSpan.Equals(other.LeftSpan) && RightSpan.Equals(other.RightSpan);

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj is BraceMatchingResult result && Equals(result);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LeftSpan.GetHashCode() * 397) ^ RightSpan.GetHashCode();
            }
        }

        public static bool operator ==(BraceMatchingResult left, BraceMatchingResult right) => left.Equals(right);

        public static bool operator !=(BraceMatchingResult left, BraceMatchingResult right) => !left.Equals(right);
    }

}
