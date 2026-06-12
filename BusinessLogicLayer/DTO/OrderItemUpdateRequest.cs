namespace BusinessLogicLayer.DTO
{
    public record OrderItemUpdateRequest(int ProductID, decimal UnitPrice, int Quantity)
    {
        public OrderItemUpdateRequest()
            : this(default, default, default) { }
    }
}
