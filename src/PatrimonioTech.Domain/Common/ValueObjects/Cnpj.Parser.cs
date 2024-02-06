using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PatrimonioTech.Domain.Common.ValueObjects;

public partial class Cnpj
{
    private static readonly FrozenSet<string> s_invalidSet = new[]
    {
        "00000000000000", "11111111111111", "22222222222222", "33333333333333", "44444444444444", "55555555555555",
        "66666666666666", "77777777777777", "88888888888888", "99999999999999"
    }.ToFrozenSet(StringComparer.Ordinal);

    private static class Parser
    {
        public static bool IsValid(string input)
        {
            return !s_invalidSet.Contains(input) && ValidateModulus(input);
        }

        private static bool ValidateModulus(ReadOnlySpan<char> value)
        {
            ReadOnlySpan<int> mult = stackalloc int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int v1 = 0;
            int v2 = 0;
            for (int i = 0; i < 12; i++)
            {
                int currentDigit = value[i] - '0';
                v1 += currentDigit * mult[i + 1];
                v2 += currentDigit * mult[i];
            }

            v1 = v1 % 11 < 2 ? 0 : 11 - (v1 % 11);
            v2 += v1 * 2;
            v2 = v2 % 11 < 2 ? 0 : 11 - (v2 % 11);

            char v1Char = (char)(v1 + '0');
            char v2Char = (char)(v2 + '0');

            return v1Char == value[12] && v2Char == value[13];
        }

        public static Result<string, CnpjError> TryNormalize(ReadOnlySpan<char> input)
        {
            Span<char> buffer =
                stackalloc char[Length] { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };

            input = input.Trim();
            if (input.Length == 0)
                return CnpjError.Empty;

            int position = buffer.Length - 1;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                char c = input[i];
                if (IsSpecialCharacter(c))
                    continue;

                if (position < 0)
                    return CnpjError.TooLong;

                if (!char.IsDigit(c))
                    return CnpjError.Invalid;

                buffer[position--] = c;
            }

            return buffer.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSpecialCharacter(char c)
        {
            return c is '.' or '-' or ' ' or '/';
        }
    }
}
