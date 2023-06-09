﻿using Account.Domain.DTOs; 

namespace Account.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IProducer<string, string> _producer;
    private readonly IMediator _mediator;
    private readonly ICustomerQueries _customerQueries;
    private readonly IOrderQueries _orderQueries;
    private readonly INetMQSocket _netMQSocket;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AccountController(
        IProducer<string, string> producer,
        IMediator mediator,
        ICustomerQueries customerQueries,
        IOrderQueries orderQueries,
        INetMQSocket netMQSocket,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _producer = producer;
        _mediator = mediator;
        _customerQueries = customerQueries;
        _orderQueries = orderQueries;
        _netMQSocket = netMQSocket;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("Order")]
    public async Task<IActionResult> Order(OrderDto order)
    { 
        order.IP = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        var orderTotal = await _mediator.Send(new OrderTotalDomainEvent(order));
        var message = await _mediator.Send(new CheckCashCustomerDomainEvent(orderTotal, order));
        _producer.Produce("order",new Message<string, string> {Key = order.CustomerId, Value = message});
         dynamic result = JsonSerializer.Deserialize<JsonElement>(_netMQSocket.ReceiveFrameString());
        var jsonResult = new
        {
            success = result.GetProperty("success").GetBoolean(),
            message = result.GetProperty("message").GetString(),
        };
        return Ok(result);
    }

    [HttpGet("GetCustomerWallet/{id}")]
    public IActionResult getCustomerWaller(string id)
    {
        decimal balance = _customerQueries.getCustomerWallet(id);
        if(balance == -1)
        {
            return BadRequest(new { message = "Khách hàng không tồn tại!" });
        }
        return Ok(new {balance = balance.ToString()});
    }

    [HttpGet("GetSuccessfulOrders")]
    public IActionResult getSuccessfulOrders()
    {
        List<Orders> orders = _orderQueries.GetOrderByStatus(1);
        if (orders.Count() != 0)
        {
            return Ok(orders);
        }
        return BadRequest(new { message = "Không có đơn hàng nào!" });
    }
    [HttpGet("GetOrderFaile")]
    public IActionResult getOrderFaile()
    {
        List<Orders> orders = _orderQueries.GetOrderByStatus(0);
        if  (orders.Count() != 0)
        {
            return Ok(orders);
        }
        return BadRequest(new { message = "Không có đơn hàng nào!" });
    }
    [HttpGet("GetOrderByCustomerID/{cus_id}")]
    public IActionResult getOrderByCustomerID(string cus_id)
    {
        List<Orders> orders = _orderQueries.getOrderByCustomerID(cus_id);
        if (orders.Count() != 0)
        {
            return Ok(orders);
        }
        return BadRequest(new { message = "Không có đơn hàng nào!" });
    }
}
