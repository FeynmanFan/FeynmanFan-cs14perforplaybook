using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using performance_helper;
using System.Text;

namespace CatalogApp.Pages
{
    public class ExportModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Result { get; set; }

        public void OnGet()
        {
            int count = 10000;

            var products = DataHelper.CreateProductsCollection(count);

            // This is the "bad" way to build a CSV string
            string csv = BuildCsvRowWithString(products);

            // we'd write this to disk or network in the real world, but that's not what we're testing
            this.Result = $"Exported {count} products";
        }

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
    }
}
