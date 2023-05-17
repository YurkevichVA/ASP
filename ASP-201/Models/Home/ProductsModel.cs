namespace ASP_201.Models.Home
{
    public class ProductsModel
    {
        public List<Product> products { get; set; } = null!;
    }
    public class Product
    {
        public String Name { get; set; } = null!;
        public Double Price { get; set; }
        public String Image { get; set; } = null!;
    }
}
