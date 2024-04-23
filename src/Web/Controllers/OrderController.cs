using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Features.MyOrders;
using Microsoft.eShopWeb.Web.Features.OrderDetails;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize] // Controllers that mainly require Authorization still use Controller/View; other pages use Pages
[Route("[controller]/[action]")]
public class OrderController : Controller
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {   
        Guard.Against.Null(User?.Identity?.Name, nameof(User.Identity.Name));
        var viewModel = await _mediator.Send(new GetMyOrders(User.Identity.Name));

        return View(viewModel);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> Detail(int orderId)
    {
        Guard.Against.Null(User?.Identity?.Name, nameof(User.Identity.Name));
        var viewModel = await _mediator.Send(new GetOrderDetails(User.Identity.Name, orderId));

        if (viewModel == null)
        {
            return BadRequest("No such order found for this user.");
        }

        return View(viewModel);
    }

    [HttpGet("term/{searchTerm}")]
    public async Task<IActionResult> Search(string searchTerm)
    {
        Guard.Against.Null(User?.Identity?.Name, nameof(User.Identity.Name));

        var viewModel = new SearchViewModel
        {
            SearchTerm = searchTerm
        };

        if (int.TryParse(searchTerm, out int searchOrderId))
        {
            var viewModelOrderDetails = await _mediator.Send(new GetOrderDetails(User.Identity.Name, searchOrderId));
            if (viewModelOrderDetails != null)
            {
                viewModel.Orders = new List<OrderViewModel>() { viewModelOrderDetails };
            }
        }

        return View(viewModel);
    }
}
