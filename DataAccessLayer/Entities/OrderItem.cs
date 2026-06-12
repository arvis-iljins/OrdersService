using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.Entities
{
    public class OrderItem
    {
        [BsonElement("ProductID")]
        [BsonRepresentation(BsonType.Int32)]
        public int ProductId { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("UnitPrice")]
        [BsonRepresentation(BsonType.Double)]
        public decimal UnitPrice { get; set; }

        [BsonElement("TotalPrice")]
        [BsonRepresentation(BsonType.Double)]
        public decimal TotalPrice { get; set; }

        [BsonElement("ProductName")]
        [BsonRepresentation(BsonType.String)]
        public string ProductName { get; set; } = string.Empty;

        [BsonElement("Category")]
        [BsonRepresentation(BsonType.String)]
        public string Category { get; set; } = string.Empty;
    }
}
