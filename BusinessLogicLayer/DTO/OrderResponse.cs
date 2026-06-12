namespace BusinessLogicLayer.DTO
{
    public record OrderResponse(
        Guid OrderID,
        Guid UserID,
        decimal TotalBill,
        DateTime OrderDate,
        string? Email,
        string? PersonName,
        List<OrderItemResponse> OrderItems
    )
    {
        public OrderResponse()
            : this(default, default, default, default, default, default, []) { }
    }
}
