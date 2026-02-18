namespace performance_helper
{
    using System.Buffers;
    using System.Text;

    public class Product
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public double Price { get; internal set; }
        public string Category { get; internal set; }
        public bool InStock { get; internal set; }

        public static string BuildCsvRowWithString(IEnumerable<Product> products)
        {
            var sb = new StringBuilder(1024);

            foreach (var product in products)
            {
                // This creates a new string on every iteration → very bad for large lists
                sb.Append(product.Id);
                sb.Append(',');
                sb.Append(product.Name);
                sb.Append(',');
                sb.Append(product.Price.ToString("F2"));
                sb.Append(',');
                sb.Append(product.Category);
                sb.Append(',');
                sb.Append(product.InStock ? "Yes" : "No");
                sb.Append('\n');
            }

            // remove the last newline for cleaner output, but this is also inefficient since it creates a new string
            // I would skip this, but the other way did it, so we need to keep it consistent for the performance comparison
            if (sb.Length > 0 && sb[sb.Length - 1] == '\n')
            {
                sb.Length--; // or sb.Length -= Environment.NewLine.Length;
            }

            return sb.ToString();
        }

        public static string BuildCsvRowWithSpan(IEnumerable<Product> products)
        {
            const int AvgLineLength = 150;           // tune higher if needed
            int estimated = products.Count() * AvgLineLength + 4096;  // generous padding

            char[] rented = ArrayPool<char>.Shared.Rent(estimated);
            try
            {
                Span<char> buffer = rented;
                int written = 0;

                foreach (var p in products)
                {
                    Append(buffer, ref written, p.Id);
                    Append(buffer, ref written, ",");
                    Append(buffer, ref written, p.Name);
                    Append(buffer, ref written, ",");

                    Span<char> priceBuf = stackalloc char[32];
                    if (p.Price.TryFormat(priceBuf, out int len, "F2"))
                    {
                        priceBuf[..len].CopyTo(buffer[written..]);
                        written += len;
                    }
                    else
                    {
                        Append(buffer, ref written, p.Price.ToString("F2"));
                    }

                    Append(buffer, ref written, ",");
                    Append(buffer, ref written, p.Category);
                    Append(buffer, ref written, ",");
                    Append(buffer, ref written, p.InStock ? "Yes" : "No");
                    Append(buffer, ref written, "\n");
                }

                if (written > 0 && buffer[written - 1] == '\n')
                    written--;

                return new string(buffer[..written]);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(rented);
            }
        }

        private static void Append(Span<char> buffer, ref int written, string s)
        {
            s.AsSpan().CopyTo(buffer[written..]);
            written += s.Length;
        }
    }
}