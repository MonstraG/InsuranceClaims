using System.Text.Json;
using System.Text.Json.Serialization;

namespace Claims.Tests;

public static class TestHelpers
{
	public static readonly JsonSerializerOptions DefaultOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		Converters = { new JsonStringEnumConverter() },
	};
}
