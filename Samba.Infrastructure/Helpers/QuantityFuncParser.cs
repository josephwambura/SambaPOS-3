using System;
using System.Linq;

namespace Samba.Infrastructure.Helpers
{
    public static class QuantityFuncParser
    {
        enum Operations
        {
            Set, Add, Subtract
        }

        public static string Parse(string quantityFunc, string currentQuantity)
        {
            return !IsFunc(quantityFunc)
                ? quantityFunc
                : !int.TryParse(currentQuantity, out int quantity) && !string.IsNullOrEmpty(currentQuantity)
                ? quantityFunc
                : Parse(quantityFunc, quantity).ToString();
        }

        private static bool IsFunc(string quantityFunc)
        {
            if (string.IsNullOrEmpty(quantityFunc)) return false;
            if (quantityFunc.Length == 1) return false;
            var operation = quantityFunc[0];
            if ("+-".All(x => x != operation)) return false;
            var value = quantityFunc.Substring(1);
            return value.All(x => ContainsChar("1234567890", x));
        }

        private static bool ContainsChar(string set, char value)
        {
            return set.ToCharArray().Any(x => x == value);
        }

        private static Operations GetFunc(string quantityFunc)
        {
            return !IsFunc(quantityFunc)
                ? Operations.Set
                : quantityFunc.StartsWith("+") ? Operations.Add : quantityFunc.StartsWith("-") ? Operations.Subtract : Operations.Set;
        }

        public static int Parse(string quantityFunc, int currentQuantity)
        {
            if (string.IsNullOrEmpty(quantityFunc)) return 0;
            var operation = GetFunc(quantityFunc);
            var trimmed = quantityFunc.Trim('-', '+', ' ');
            Int32.TryParse(trimmed, out int value);
            return operation == Operations.Add ? currentQuantity + value : operation == Operations.Subtract ? currentQuantity - value : value;
        }
    }
}