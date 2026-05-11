using Blog;
using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using static Blog.BlogService;

namespace server
{
    internal class BlogServiceImpl : BlogServiceBase
    {
        private static MongoClient _mongoClient = new("mongodb://localhost:27017");
        private static IMongoDatabase _mongoDatabase = _mongoClient.GetDatabase("mydb");
        private static IMongoCollection<BsonDocument> _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("blog");

        public override Task<CreateBlogResponse> CreateBlog(CreateBlogRequest request, ServerCallContext context)
        {
            var blog = request.Blog;
            BsonDocument doc = new BsonDocument("author_id", blog.AuthorId)
                                                .Add("title", blog.Title)
                                                .Add("content", blog.Content);

            _mongoCollection.InsertOne(doc);

            var id = doc.GetValue("_id").ToString();

            blog.Id = id;

            return Task.FromResult(new CreateBlogResponse()
            {
                Blog = blog
            });
        }

        public override async Task<ReadBlogResponse> ReadBlog(ReadBlogRequest request, ServerCallContext context)
        {
            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(request.BlogId));
            var result = _mongoCollection.Find(filter).FirstOrDefault();

            if (result is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Blog with id " + request.BlogId + " wasn't find."));

            Blog.Blog blog = new Blog.Blog()
            {
                AuthorId = result.GetValue("author_id").AsString,
                Title = result.GetValue("title").AsString,
                Content = result.GetValue("content").AsString,
            };

            return new ReadBlogResponse() { Blog = blog };
        }
    }
}