﻿namespace MovieApplication.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
