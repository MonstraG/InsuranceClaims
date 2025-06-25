using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Claims.Models;

public class Cover : IIdentifiable
{
	[BsonId]
	public string Id { get; set; }

	[BsonElement("startDate")]
	// todo: re-add, breaks serialization
	// [BsonDateTimeOptions(DateOnly = true)]
	public DateTime StartDate { get; set; }

	[BsonElement("endDate")]
	// todo: re-add, breaks serialization
	// [BsonDateTimeOptions(DateOnly = true)]
	public DateTime EndDate { get; set; }

	[BsonElement("claimType")]
	public CoverType Type { get; set; }

	[BsonElement("premium")]
	public decimal Premium { get; set; }
}

public enum CoverType
{
	Yacht = 0,
	PassengerShip = 1,
	ContainerShip = 2,
	BulkCarrier = 3,
	Tanker = 4,
}
