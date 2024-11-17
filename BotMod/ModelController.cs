using System.Data;

namespace BotMod
{
    public class ModelController
    {
        private List<Model> models = new List<Model>();
        private BotModDBConnector dbConnector = new BotModDBConnector("Server=localhost:5432;Database=bot_mod;User ID=postgres;Password=1488;");
        public ModelController() { }
        public void CreateModel(string name, int age, bool isPlusSize, string contact, string sex, string comment)
        {
            var model = new Model(Guid.NewGuid(), name, age, isPlusSize, contact, sex, comment);
            models.Add(model);
            dbConnector.CreateModel(model);
        }

        public List<Model> GetModelsBySexOrAge(string? sex, int? ageFrom, int? ageTo)
        {
            try
            {
                var models = dbConnector.GetAllModels();
                var modelsResponse = new List<Model>();

                //ageFrom
                if (ageFrom is not null && (modelsResponse.Count() != 0))
                {
                    modelsResponse = modelsResponse.Where(m => m.Age >= ageFrom).ToList();

                    //var filteredModels = new List<Model>();
                    //filteredModels.AddRange(models.Where(m => m.Age >= filter.AgeFrom));
                    //modelsResponse = filteredModels;
                }
                else if (ageFrom is not null && (modelsResponse.Count() == 0))
                {
                    modelsResponse.AddRange(models.Where(m => m.Age >= ageFrom));
                }

                //ageTo
                if (ageTo is not null && (modelsResponse.Count() != 0))
                {
                    modelsResponse = modelsResponse.Where(m => m.Age <= ageTo).ToList();

                    /*
                    var filteredModels = new List<Model>();
                    filteredModels.AddRange(models.Where(m => m.Age <= filter.AgeTo));
                    modelsResponse = filteredModels;
                    */
                }
                else if (ageTo is not null && (modelsResponse.Count() == 0))
                {
                    modelsResponse.AddRange(models.Where(m => m.Age <= ageTo));
                }

                //sex
                if (sex is not null && (modelsResponse.Count() != 0))
                {
                    modelsResponse = modelsResponse.Where(m => m.Sex.Equals(sex)).ToList();

                    /*
                    var filteredModels = new List<Model>();
                    filteredModels.AddRange(models.Where(m => m.Sex.Equals(filter.Sex)));
                    modelsResponse = filteredModels;
                    */
                }
                else if (sex is not null && (modelsResponse.Count() == 0))
                {
                    modelsResponse.AddRange(models.Where(m => m.Sex.Equals(sex)));
                }

                //modelsResponse.DistinctBy(m => m.Id);

                return modelsResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public List<Model> GetModel(ModelFilter filter)
        {
            try
            {
                var modelsResponse = new List<Model>();

                //name
                if (filter.Name is not null)
                {
                    modelsResponse.AddRange(models.Where(m => m.Name.Equals(filter.Name)));
                }

                //ageFrom
                if (filter.AgeFrom is not null && modelsResponse is not null)
                {
                    modelsResponse = modelsResponse.Where(m => m.Age >= filter.AgeFrom).ToList();

                    //var filteredModels = new List<Model>();
                    //filteredModels.AddRange(models.Where(m => m.Age >= filter.AgeFrom));
                    //modelsResponse = filteredModels;
                }
                else if (filter.AgeFrom is not null && modelsResponse is null)
                {
                    modelsResponse.AddRange(models.Where(m => m.Age >= filter.AgeFrom));
                }

                //ageTo
                if (filter.AgeTo is not null && modelsResponse is not null)
                {
                    modelsResponse = modelsResponse.Where(m => m.Age <= filter.AgeTo).ToList();

                    /*
                    var filteredModels = new List<Model>();
                    filteredModels.AddRange(models.Where(m => m.Age <= filter.AgeTo));
                    modelsResponse = filteredModels;
                    */
                }
                else if (filter.AgeTo is not null && modelsResponse is null)
                {
                    modelsResponse.AddRange(models.Where(m => m.Age <= filter.AgeTo));
                }

                //plusSize
                if (filter.IsPlusSize is not null && modelsResponse is not null)
                {
                    modelsResponse = modelsResponse.Where(m => m.IsPlusSize.Equals(filter.IsPlusSize)).ToList();

                    /*
                    var filteredModels = new List<Model>();
                    filteredModels.AddRange(models.Where(m => m.IsPlusSize.Equals(true)));
                    modelsResponse = filteredModels;
                    */
                }
                else if (filter.IsPlusSize is not null && modelsResponse is null)
                {
                    modelsResponse.AddRange(models.Where(m => m.IsPlusSize.Equals(filter.IsPlusSize)));
                }

                //sex
                if (filter.Sex is not null && modelsResponse is not null)
                {
                    modelsResponse = modelsResponse.Where(m => m.Sex.Equals(filter.Sex)).ToList();

                    /*
                    var filteredModels = new List<Model>();
                    filteredModels.AddRange(models.Where(m => m.Sex.Equals(filter.Sex)));
                    modelsResponse = filteredModels;
                    */
                }
                else if (filter.Sex is not null && modelsResponse is null)
                {
                    modelsResponse.AddRange(models.Where(m => m.Sex.Equals(filter.Sex)));
                }

                //modelsResponse.DistinctBy(m => m.Id);

                return modelsResponse;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public void DeleteModel(Guid id)
        {
            try
            {
                //var deletedModel = models.Where(m => m.Id == id).FirstOrDefault();
                //var deletedModelsId = models.IndexOf(deletedModel);
                //models.RemoveAt(deletedModelsId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public void EditModel(Guid oldModelId, Model newModel)
        {
            DeleteModel(oldModelId);
            //CreateModel(newModel);
        }
    }
}
