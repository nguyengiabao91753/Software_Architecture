using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Controllers;
using Orders.Application.Dtos;
using Orders.Application.IServices;
using Orders.Domain.Enums;

namespace Orders.API.Tests.Controllers;

public class OrderAPIControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly OrderAPIController _controller;

    public OrderAPIControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrderAPIController(_mockOrderService.Object);
    }

    #region Get Tests

    [Fact]
    public async Task Get_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: Guid.NewGuid(),
            VoucherId: null,
            TotalAmount: 100.00m,
            OrderStatus: OrderStatus.Pending.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var result = new ResultService<OrderDto>
        {
            Data = orderDto,
            IsSuccess = true,
            Message = "Order found successfully"
        };

        _mockOrderService.Setup(s => s.GetById(orderId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Get(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
        var returnedResult = okResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.True(returnedResult.IsSuccess);
        Assert.Equal(orderId, returnedResult.Data?.Id);
        _mockOrderService.Verify(s => s.GetById(orderId), Times.Once);
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var result = new ResultService<OrderDto>
        {
            Data = null,
            IsSuccess = false,
            Message = "Order not found"
        };

        _mockOrderService.Setup(s => s.GetById(orderId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.Get(orderId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.NotNull(badRequestResult.Value);
        var returnedResult = badRequestResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.False(returnedResult.IsSuccess);
        _mockOrderService.Verify(s => s.GetById(orderId), Times.Once);
    }

    [Fact]
    public async Task Get_WithInvalidGuidFormat_ThrowsFormatException()
    {
        // Arrange
        var invalidId = "invalid-guid";

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => _controller.Get(Guid.Parse(invalidId)));
    }

    #endregion

    #region GetByTracking Tests

    [Fact]
    public async Task GetByTracking_WithValidTrackingId_ReturnsOkResult()
    {
        // Arrange
        var trackingId = Guid.NewGuid();
        var orderDto = new OrderDto(
            Id: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: trackingId,
            VoucherId: null,
            TotalAmount: 150.00m,
            OrderStatus: OrderStatus.Approved.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var result = new ResultService<OrderDto>
        {
            Data = orderDto,
            IsSuccess = true,
            Message = "Order tracking found"
        };

        _mockOrderService.Setup(s => s.GetByTrackingId(trackingId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetByTracking(trackingId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedResult = okResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.True(returnedResult.IsSuccess);
        Assert.Equal(trackingId, returnedResult.Data?.TrackingId);
        _mockOrderService.Verify(s => s.GetByTrackingId(trackingId), Times.Once);
    }

    [Fact]
    public async Task GetByTracking_WithNonExistentTrackingId_ReturnsBadRequest()
    {
        // Arrange
        var trackingId = Guid.NewGuid();
        var result = new ResultService<OrderDto>
        {
            Data = null,
            IsSuccess = false,
            Message = "Tracking ID not found"
        };

        _mockOrderService.Setup(s => s.GetByTrackingId(trackingId))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.GetByTracking(trackingId.ToString());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var returnedResult = badRequestResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.False(returnedResult.IsSuccess);
    }

    #endregion

    #region CreateOrder Tests

    [Fact]
    public async Task CreateOrder_WithValidOrderDto_ReturnsOkResult()
    {
        // Arrange
        var orderDto = new OrderDto(
            Id: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: Guid.NewGuid(),
            VoucherId: null,
            TotalAmount: 200.00m,
            OrderStatus: OrderStatus.Pending.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var result = new ResultService<OrderDto>
        {
            Data = orderDto,
            IsSuccess = true,
            Message = "Order created successfully"
        };

        _mockOrderService.Setup(s => s.Save(orderDto))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.CreateOrder(orderDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedResult = okResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.True(returnedResult.IsSuccess);
        Assert.Equal(orderDto.Id, returnedResult.Data?.Id);
        _mockOrderService.Verify(s => s.Save(orderDto), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidOrderDto_ReturnsBadRequest()
    {
        // Arrange
        var orderDto = new OrderDto(
            Id: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: Guid.NewGuid(),
            VoucherId: null,
            TotalAmount: 0, // Invalid amount
            OrderStatus: OrderStatus.Pending.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var result = new ResultService<OrderDto>
        {
            Data = null,
            IsSuccess = false,
            Message = "Invalid order data"
        };

        _mockOrderService.Setup(s => s.Save(orderDto))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.CreateOrder(orderDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var returnedResult = badRequestResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.False(returnedResult.IsSuccess);
        _mockOrderService.Verify(s => s.Save(orderDto), Times.Once);
    }

    #endregion

    #region ApproveOrder Tests

    [Fact]
    public async Task ApproveOrder_WithValidOrderDto_ReturnsOkResultWithApprovedStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: Guid.NewGuid(),
            VoucherId: null,
            TotalAmount: 100.00m,
            OrderStatus: OrderStatus.Pending.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var updatedOrderDto = orderDto with { OrderStatus = OrderStatus.Approved.ToString() };

        var result = new ResultService<OrderDto>
        {
            Data = updatedOrderDto,
            IsSuccess = true,
            Message = "Order approved successfully"
        };

        _mockOrderService.Setup(s => s.Update(It.Is<OrderDto>(o => o.OrderStatus == OrderStatus.Approved.ToString())))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.ApproveOrder(orderDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedResult = okResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.True(returnedResult.IsSuccess);
        Assert.Equal(OrderStatus.Approved.ToString(), returnedResult.Data?.OrderStatus);
        _mockOrderService.Verify(s => s.Update(It.IsAny<OrderDto>()), Times.Once);
    }

    [Fact]
    public async Task ApproveOrder_WithServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var orderDto = new OrderDto(
            Id: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: Guid.NewGuid(),
            VoucherId: null,
            TotalAmount: 100.00m,
            OrderStatus: OrderStatus.Pending.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var result = new ResultService<OrderDto>
        {
            Data = null,
            IsSuccess = false,
            Message = "Failed to approve order"
        };

        _mockOrderService.Setup(s => s.Update(It.IsAny<OrderDto>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _controller.ApproveOrder(orderDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var returnedResult = badRequestResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.False(returnedResult.IsSuccess);
    }

    #endregion

    #region PayOrder Tests

    [Fact]
    public async Task PayOrder_WithValidOrderId_ReturnsOkResultWithPaidStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: Guid.NewGuid(),
            VoucherId: null,
            TotalAmount: 100.00m,
            OrderStatus: OrderStatus.Approved.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var getResult = new ResultService<OrderDto>
        {
            Data = orderDto,
            IsSuccess = true,
            Message = "Order found"
        };

        var updatedOrderDto = orderDto with { OrderStatus = OrderStatus.Paid.ToString() };
        var updateResult = new ResultService<OrderDto>
        {
            Data = updatedOrderDto,
            IsSuccess = true,
            Message = "Order paid successfully"
        };

        _mockOrderService.Setup(s => s.GetById(orderId))
            .ReturnsAsync(getResult);

        _mockOrderService.Setup(s => s.Update(It.Is<OrderDto>(o => o.OrderStatus == OrderStatus.Paid.ToString())))
            .ReturnsAsync(updateResult);

        // Act
        var actionResult = await _controller.PayOrder(orderId.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedResult = okResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.True(returnedResult.IsSuccess);
        Assert.Equal(OrderStatus.Paid.ToString(), returnedResult.Data?.OrderStatus);
        _mockOrderService.Verify(s => s.GetById(orderId), Times.Once);
        _mockOrderService.Verify(s => s.Update(It.IsAny<OrderDto>()), Times.Once);
    }

    [Fact]
    public async Task PayOrder_WithNonExistentOrderId_ReturnsBadRequest()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var getResult = new ResultService<OrderDto>
        {
            Data = null,
            IsSuccess = false,
            Message = "Order not found"
        };

        _mockOrderService.Setup(s => s.GetById(orderId))
            .ReturnsAsync(getResult);

        // Act
        var actionResult = await _controller.PayOrder(orderId.ToString());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var returnedResult = badRequestResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.False(returnedResult.IsSuccess);
        _mockOrderService.Verify(s => s.GetById(orderId), Times.Once);
        _mockOrderService.Verify(s => s.Update(It.IsAny<OrderDto>()), Times.Never);
    }

    [Fact]
    public async Task PayOrder_WithUpdateFailure_ReturnsBadRequest()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: Guid.NewGuid(),
            RestaurantId: Guid.NewGuid(),
            TrackingId: Guid.NewGuid(),
            VoucherId: null,
            TotalAmount: 100.00m,
            OrderStatus: OrderStatus.Approved.ToString(),
            FailureMessages: null,
            OrderItems: new List<OrderItemDto>()
        );

        var getResult = new ResultService<OrderDto>
        {
            Data = orderDto,
            IsSuccess = true,
            Message = "Order found"
        };

        var updateResult = new ResultService<OrderDto>
        {
            Data = null,
            IsSuccess = false,
            Message = "Failed to update order payment status"
        };

        _mockOrderService.Setup(s => s.GetById(orderId))
            .ReturnsAsync(getResult);

        _mockOrderService.Setup(s => s.Update(It.IsAny<OrderDto>()))
            .ReturnsAsync(updateResult);

        // Act
        var actionResult = await _controller.PayOrder(orderId.ToString());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var returnedResult = badRequestResult.Value as ResultService<OrderDto>;
        Assert.NotNull(returnedResult);
        Assert.False(returnedResult.IsSuccess);
    }

    [Fact]
    public async Task PayOrder_WithInvalidGuidFormat_ThrowsFormatException()
    {
        // Arrange
        var invalidId = "invalid-guid";

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => _controller.PayOrder(invalidId));
    }

    #endregion
}
