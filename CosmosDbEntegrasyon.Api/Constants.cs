namespace CosmosDbEntegrasyon.Api;

public class Constants
{
    public static readonly string[] ExcludedProperties = {
        "PageSize",
        "PageNumber",
        "CompanyId",
        "MinPrice",
        "MaxPrice"
    };
    
    public static string ProductsContainerName = "Products";
}