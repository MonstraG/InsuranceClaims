using System.Text.Json.Serialization;
using Claims.Claims.DTOs;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Claims.Models;

public class Claim : IIdentifiable
{
	public const double MaxCost = 100_000d;

	[JsonConstructor]
	private Claim() { }

	public Claim(NewClaimDTO newClaim)
	{
		Id = Guid.NewGuid().ToString();
		CoverId = newClaim.CoverId;
		Created = newClaim.Created;
		Name = newClaim.Name;
		Type = newClaim.Type;
		DamageCost = newClaim.DamageCost;
	}

	[BsonId]
	public string Id { get; set; } = null!;

	[BsonElement("coverId")]
	public string CoverId { get; set; } = null!;

	[BsonElement("created")]
	// todo: re-add, breaks serialization
	// [BsonDateTimeOptions(DateOnly = true)]
	public DateTime Created { get; set; }

	[BsonElement("name")]
	public string Name { get; set; } = null!;

	[BsonElement("claimType")]
	public ClaimType Type { get; set; }

	[BsonElement("damageCost")]
	public decimal DamageCost { get; set; }
}

public enum ClaimType
{
	Collision = 0,
	Grounding = 1,
	BadWeather = 2,
	Fire = 3,
}
