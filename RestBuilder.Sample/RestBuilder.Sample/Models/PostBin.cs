using System;
namespace RestBuilder.Sample
{
    public class PostBin
    {
        public string binId { get; set; } = DefaultValue;
        public long inserted { get; set; }
        public long updated { get; set; }
        public long expires { get; set; }

        public const string DefaultValue = "Unassigned";

        public override string ToString()
        {
            return $"PostBin: {binId}";
        }
    }
}
