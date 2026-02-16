using performance_helper;

namespace performance_testing
{
    public class UnitTest1
    {
        [Fact]
        public void CreateProductsFile()
        {
            // NOT a real unit test, just a convenient place to run this code once to generate the file for testing

            DataHelper.CreateTabDelimitedProductsFile(5000);
        }
    }
}
