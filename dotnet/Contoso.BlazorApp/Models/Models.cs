using System.Text.Json.Serialization;

namespace Contoso.BlazorApp.Models;

public class Post
{
    [JsonPropertyName("id")]
    public int? PostId { get; set; }
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedDate { get; set; }
    
    [JsonPropertyName("likesCount")]
    public int? LikeCount { get; set; }
    
    [JsonPropertyName("commentsCount")]
    public int? CommentCount { get; set; }
    
    [JsonPropertyName("is_liked")]
    public bool? Liked { get; set; }
}

public class Comment
{
    [JsonPropertyName("id")]
    public int? CommentId { get; set; }
    
    [JsonPropertyName("post_id")]
    public int? PostId { get; set; }
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedDate { get; set; }
}

public class User
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("userId")]
    public int? UserId { get; set; }
}

public class ApiResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
