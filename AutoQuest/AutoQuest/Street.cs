using System.Runtime.Serialization;

namespace AutoQuest
{
    [DataContract]
    public class Street
    {
        public Street()
        {
        }

        [DataMember(IsRequired = true, Order = 1)]
        public string Name { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public string Type { get;  set; }

        [DataMember(IsRequired = false, Order = 3, EmitDefaultValue = false)]
        public string District { get; private set; }

        [DataMember(IsRequired = true, Order = 4)]
        public double Long { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public double Lat { get; set; }

        [DataMember(IsRequired = false, Order = 6, EmitDefaultValue = false)]
        public string AltName { get; private set; }

        [DataMember(IsRequired = false, Order = 7, EmitDefaultValue = false)]
        public string AltType { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var representation = District != null ? $"{Name} {Type}, {District}" : $"{Name} {Type}";
            if (AltName != null || AltType != null)
                representation += $"({AltName ?? Name}, {AltType ?? Type})";
            return representation;
        }
    }
}
