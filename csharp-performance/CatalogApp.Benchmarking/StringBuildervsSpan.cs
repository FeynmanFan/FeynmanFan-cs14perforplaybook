using BenchmarkDotNet.Attributes;
using performance_helper;

namespace CatalogApp.Benchmarking
{
    [ShortRunJob()]
    [MemoryDiagnoser]
    public class StringBuildervsSpan
    {
        private IEnumerable<Product> products;

        [Params(1000, 10000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            products = DataHelper.CreateProductsCollection(N);
        }

        [Benchmark]
        public string WithStringBuilder() => Product.BuildCsvRowWithString(products);

        [Benchmark]
        public string WithSpan() => Product.BuildCsvRowWithSpan(products);
    }
}
