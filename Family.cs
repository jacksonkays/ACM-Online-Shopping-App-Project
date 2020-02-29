using Newtonsoft.Json;

namespace ACM_Online_Shopping_App
{
    internal class Family
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ItemType { get; set; }
        public Shirts[] T_Shirts { get; set; }
        public Shirts[] Dress_Shirts { get; set; }
        public Pants[] Jeans { get; set; }
        public Pants[] Dress_Pants { get; set; }
        public bool inSystem { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class Shirts
    {
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Price { get; set; }
        internal class T_Shirts : Shirts
        {
        }
        internal class Dress_Shirts : Shirts
        {
        }
    }
    public class Pants
    {
        public string PantType { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Price { get; set; }
        internal class Jeans : Pants { }
        internal class Dress_Pants : Pants { }
    }
}