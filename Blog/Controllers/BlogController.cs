using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Blog.Models;


namespace Blog.Controllers
{
    //Route the http requests to the specified URL
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        //Creating a local json data storage file
        private const string DataFilePath = "blog_data.json";

        //Get all blog posts 
        [HttpGet("/posts")]
        public IActionResult GetPosts()
        {
            try
            {
                var posts = ReadData();
                return Ok(posts.OrderByDescending(p => p.TimeStamp));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //Get blog posts by id 
        [HttpGet("/posts/{id}")]
        public IActionResult GetPost(int id)
        {
            //Error handling for internal server errors
            try
            {
                var posts = ReadData();
                var post = posts.FirstOrDefault(p => p.Id == id);

                if (post == null)
                {
                    return NotFound();
                }

                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //Get all available categories
        [HttpGet("/categories")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = new List<CategoryModel>
                {
                    new CategoryModel { Id = 1, Name = "General" },
                    new CategoryModel { Id = 2, Name = "Technology" },
                    new CategoryModel { Id = 3, Name = "Random" }
                };

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //Create a new post by passing title, category id and content 
        [HttpPost("/posts")]
        public IActionResult CreatePost(PostModel newPost)
        {
            try
            {
                var posts = ReadData();
                newPost.Id = posts.Count + 1;
                newPost.TimeStamp = DateTime.Now;

                posts.Add(newPost);
                WriteData(posts);

                return CreatedAtAction(nameof(GetPost), new { id = newPost.Id }, newPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //Update a post by id 
        [HttpPut("/posts/{id}")]
        public IActionResult UpdatePost(int id, PostModel updatedPost)
        {
            try
            {
                var posts = ReadData();
                var existingPost = posts.FirstOrDefault(p => p.Id == id);

                if (existingPost == null)
                {
                    return NotFound();
                }

                existingPost.Title = updatedPost.Title;
                existingPost.Contents = updatedPost.Contents;
                existingPost.CategoryId = updatedPost.CategoryId;

                WriteData(posts);

                return Ok(existingPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //Delete all posts 
        [HttpDelete("/posts")]
        public IActionResult DeleteAllPosts()
        {
            try
            {
                WriteData(new List<PostModel>());
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //Delete a post by id 
        [HttpDelete("/posts/{id}")]
        public IActionResult DeletePost(int id)
        {
            try
            {
                var posts = ReadData();
                var postToRemove = posts.FirstOrDefault(p => p.Id == id);

                if (postToRemove == null)
                {
                    return NotFound();
                }

                posts.Remove(postToRemove);
                WriteData(posts);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //Helper functions to read and write data into the local json file 
        private List<PostModel> ReadData()
        {
            try
            {
                if (!System.IO.File.Exists(DataFilePath))
                {
                    return new List<PostModel>();
                }

                var jsonData = System.IO.File.ReadAllText(DataFilePath);
                return JsonConvert.DeserializeObject<List<PostModel>>(jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading data: {ex.Message}");
            }
        }

        private void WriteData(List<PostModel> posts)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(posts, Formatting.Indented);
                System.IO.File.WriteAllText(DataFilePath, jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing data: {ex.Message}");
            }
        }
    }
}
