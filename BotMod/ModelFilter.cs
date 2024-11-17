namespace BotMod
{
    public class ModelFilter
    {
        public string? Name { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public bool? IsPlusSize { get; set; }
        public string? Sex { get; set; }

        public ModelFilter(string? name, int? ageFrom, int? ageTo, bool? isPlusSize, string? sex)
        {
            Name = name;
            AgeFrom = ageFrom;
            AgeTo = ageTo;
            IsPlusSize = isPlusSize;
            Sex = sex;
        }
    }
}
