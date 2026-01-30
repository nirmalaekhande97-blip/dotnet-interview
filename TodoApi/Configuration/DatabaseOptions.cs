namespace TodoApi.Configuration
{
    /// <summary>
    /// Configuration options for database connection
    /// </summary>
    public class DatabaseOptions
    {
        public const string SectionName = "Database";

        public string ConnectionString { get; set; } = "Data Source=todos.db";
    }
}
