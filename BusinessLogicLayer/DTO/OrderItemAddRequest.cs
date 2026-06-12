namespace BusinessLogicLayer.DTO
{
    public record OrderItemAddRequest(int ProductID, decimal UnitPrice, int Quantity)
    {
        public OrderItemAddRequest()
            : this(default, default, default) { }
    }
}
