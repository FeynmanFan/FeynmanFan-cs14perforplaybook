namespace CatalogApp.Pages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using performance_helper;

    public class ExportModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Result { get; set; }

        public void OnGet()
        {
            int count = 10000;

            var products = DataHelper.CreateProductsCollection(count);

            // This is the "bad" way to build a CSV string
            string csv = Product.BuildCsvRowWithString(products);

            // we'd write this to disk or network in the real world, but that's not what we're testing
            this.Result = $"Exported {count} products";
        }
    }
}
