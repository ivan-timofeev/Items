using System.Text;
using System.Text.Json.Serialization;

namespace Items.Models.DataTransferObjects
{
    public sealed class FilterDto
    {
        [JsonPropertyName("selectedCategories")]
        public required IEnumerable<string> SelectedCategories { get; init; }

        [JsonPropertyName("selectedPriceRange")]
        public required PriceRange SelectedPriceRange { get; init; }

        public override string ToString()
        {
            var selectedCategories = $"cat={string.Join(',', SelectedCategories)};";
            var selectedPriceRange = $"f={SelectedPriceRange.From},to={SelectedPriceRange.To};";
            return $"{{{selectedCategories}{selectedPriceRange}}}";
        }
    }

    public sealed class PriceRange
    {
        [JsonPropertyName("from")]
        public decimal? From { get; init; }

        [JsonPropertyName("to")]
        public decimal? To { get; init; }
    }
}
