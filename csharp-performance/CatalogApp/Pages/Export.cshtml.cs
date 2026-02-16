using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using performance_helper;

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
            string result = "";

            foreach (var product in products)
            {
                // This creates a new string on every iteration → very bad for large lists
                result += product.Id + "," +
                          product.Name + "," +
                          product.Price.ToString("F2") + "," +
                          product.Category + "," +
                          (product.InStock ? "Yes" : "No") + "\n";
            }

            // Often people also do this at the end
            if (result.EndsWith("\n"))
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }
    }
}
