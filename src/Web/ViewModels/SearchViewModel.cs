namespace Microsoft.eShopWeb.Web.ViewModels;

public class SearchViewModel
{
    public string? SearchTerm { get; set; }
    public IList<OrderViewModel>? Orders { get; set; }
}
