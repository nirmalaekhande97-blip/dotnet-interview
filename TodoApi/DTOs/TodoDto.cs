namespace TodoApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for TODO item responses
    /// </summary>
    public class TodoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
