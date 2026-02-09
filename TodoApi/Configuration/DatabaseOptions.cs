namespace TodoApi.Configuration
{
    public class DatabaseOptions
    {
        public const string SectionName = "Database";
        public string ConnectionString { get; set; } = "Data Source=todos.db";
    }
}
