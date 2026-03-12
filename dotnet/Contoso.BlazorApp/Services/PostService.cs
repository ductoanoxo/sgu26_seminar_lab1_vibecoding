using Contoso.BlazorApp.Models;

namespace Contoso.BlazorApp.Services;

public class PostService
{
    private readonly ApiClientService _apiClient;

    public PostService(ApiClientService apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResponse<List<Post>>?> GetPostsAsync()
    {
        var result = await _apiClient.GetAsync<List<Post>>("/posts");
        return new ApiResponse<List<Post>> { Data = result };
    }

    public async Task<ApiResponse<Post>?> GetPostAsync(int postId)
    {
        var result = await _apiClient.GetAsync<Post>($"/posts/{postId}");
        return new ApiResponse<Post> { Data = result };
    }

    public async Task<ApiResponse<Post>?> CreatePostAsync(string content, string username)
    {
        var data = new { username, content };
        var result = await _apiClient.PostAsync<Post>("/posts", data);
        return new ApiResponse<Post> { Data = result };
    }

    public async Task<ApiResponse<Post>?> UpdatePostAsync(int postId, string content, string username)
    {
        var data = new { username, content };
        var result = await _apiClient.PatchAsync<Post>($"/posts/{postId}", data);
        return new ApiResponse<Post> { Data = result };
    }

    public async Task DeletePostAsync(int postId)
    {
        await _apiClient.DeleteAsync($"/posts/{postId}");
    }

    public async Task<ApiResponse<Post>?> LikePostAsync(int postId, string username)
    {
        var data = new { username };
        var result = await _apiClient.PostAsync<Post>($"/posts/{postId}/likes", data);
        return new ApiResponse<Post> { Data = result };
    }

    public async Task<ApiResponse<Post>?> UnlikePostAsync(int postId)
    {
        var result = await _apiClient.DeleteAsync<Post>($"/posts/{postId}/likes");
        return new ApiResponse<Post> { Data = result };
    }
}

public class CommentService
{
    private readonly ApiClientService _apiClient;

    public CommentService(ApiClientService apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ApiResponse<List<Comment>>?> GetCommentsAsync(int postId)
    {
        var result = await _apiClient.GetAsync<List<Comment>>($"/posts/{postId}/comments");
        return new ApiResponse<List<Comment>> { Data = result };
    }

    public async Task<ApiResponse<Comment>?> CreateCommentAsync(int postId, string content, string username)
    {
        var data = new { username, content };
        var result = await _apiClient.PostAsync<Comment>($"/posts/{postId}/comments", data);
        return new ApiResponse<Comment> { Data = result };
    }

    public async Task<ApiResponse<Comment>?> GetCommentAsync(int postId, int commentId)
    {
        var result = await _apiClient.GetAsync<Comment>($"/posts/{postId}/comments/{commentId}");
        return new ApiResponse<Comment> { Data = result };
    }

    public async Task<ApiResponse<Comment>?> UpdateCommentAsync(int postId, int commentId, string content, string username)
    {
        var data = new { username, content };
        var result = await _apiClient.PatchAsync<Comment>($"/posts/{postId}/comments/{commentId}", data);
        return new ApiResponse<Comment> { Data = result };
    }

    public async Task DeleteCommentAsync(int postId, int commentId)
    {
        await _apiClient.DeleteAsync($"/posts/{postId}/comments/{commentId}");
    }
}
