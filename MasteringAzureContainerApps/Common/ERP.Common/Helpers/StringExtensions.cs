using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;

namespace ERP.Common.Helpers
{
    public static class StringExtensions
    {
        public static string ToSha256Hex(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var hex = ToLowerHex(hash);
            return hex;
        }

        public static string ToXxHash128Hex(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            ReadOnlySpan<byte> inputBytes = Encoding.UTF8.GetBytes(input);

            // .Net exposes 128-bit as 1-byte array
            byte[] hash = XxHash128.Hash(inputBytes);
            var hex = ToLowerHex(hash);
            return hex;
        }

        private static string ToLowerHex(ReadOnlySpan<byte> bytes)
        {
            //Allocate exactly 2 chars per byte
            char[] c = ArrayPool<char>.Shared.Rent(bytes.Length * 2);
            try
            {
                int ci = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte b = bytes[i];
                    c[ci++] = NibbleToHex(b >> 4);
                    c[ci++] = NibbleToHex(b & 0xF);
                }
                return new string(c, 0, bytes.Length * 2);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(c);
            }

            static char NibbleToHex(int nibble) => (char)(nibble < 10 ? '0' + nibble : 'a' + (nibble - 10));
        }
    }
}
