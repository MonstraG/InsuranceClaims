using System.Text.Json.Serialization;
using Claims.Claims.DTOs;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Claims.Models;

public class Cover : IIdentifiable, ICover
{
	[JsonConstructor]
	private Cover() { }

	public Cover(NewCoverDTO newCover)
	{
		Id = Guid.NewGuid().ToString();
		StartDate = newCover.StartDate;
		EndDate = newCover.EndDate;
		Type = newCover.Type;
		Premium = PremiumComputer.ComputePremium(newCover);
	}

	[BsonId]
	public string Id { get; set; } = null!;

	[BsonElement("startDate")]
	// todo: re-add, breaks serialization
	// [BsonDateTimeOptions(DateOnly = true)]
	public DateTime StartDate { get; set; }

	[BsonElement("endDate")]
	// todo: re-add, breaks serialization
	// [BsonDateTimeOptions(DateOnly = true)]
	public DateTime EndDate { get; set; }

	[BsonElement("claimType")]
	public Types Type { get; set; }

	[BsonElement("premium")]
	public decimal Premium { get; set; }

	public enum Types
	{
		Yacht = 0,
		PassengerShip = 1,
		ContainerShip = 2,
		BulkCarrier = 3,
		Tanker = 4,
	}
}
