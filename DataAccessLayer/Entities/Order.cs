using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.Entities
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid _id { get; set; }

        [BsonElement("OrderID")]
        [BsonRepresentation(BsonType.String)]
        public Guid OrderId { get; set; }

        [BsonElement("UserID")]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        [BsonElement("OrderDate")]
        public DateTime OrderDate { get; set; }

        [BsonElement("TotalBill")]
        [BsonRepresentation(BsonType.Double)]
        public decimal TotalAmount { get; set; }

        [BsonElement("OrderItems")]
        public List<OrderItem> Items { get; set; } = [];

        [BsonIgnore]
        public string? UserEmail { get; set; }

        [BsonIgnore]
        public string? UserPersonName { get; set; }
    }
}
