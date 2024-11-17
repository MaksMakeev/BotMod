namespace BotMod
{
    public class Model
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsPlusSize { get; set; }
        public string Contact { get; set; }
        public string Sex { get; set; }
        public string Comment { get; set; }
        public Model(Guid id, string name, int age, bool isPlusSize, string contact, string sex, string comment)
        {
            Id = id;
            Name = name;
            Age = age;
            IsPlusSize = isPlusSize;
            Contact = contact;
            Sex = sex;
            Comment = comment;
        }
    }
}
