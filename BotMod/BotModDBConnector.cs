using Dapper;
using Npgsql;

namespace BotMod
{
    internal class BotModDBConnector
    {
        private readonly string _connectionString;

        public BotModDBConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateModel(Model model)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO model2(Id, Name, Age, IsPlusSize, Contact, Sex, Comment) VALUES (@Id, @Name, @Age, @IsPlusSize, @Contact, @Sex, @Comment);";
                connection.Execute(query, model);
            }
        }

        public List<Model> GetAllModels()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM model2";
                return connection.Query<Model>(query).AsList();
            }
        }

    }
}
