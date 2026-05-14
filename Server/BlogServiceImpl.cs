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

            Blog.Blog blog = new()
            {
                AuthorId = result.GetValue("author_id").AsString,
                Title = result.GetValue("title").AsString,
                Content = result.GetValue("content").AsString,
            };

            return new ReadBlogResponse() { Blog = blog };
        }

        public override async Task<UpdateBlogResponse> UpdateBlog(UpdateBlogRequest request, ServerCallContext context)
        {
            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(request.Blog.Id));
            var result = _mongoCollection.Find(filter).FirstOrDefault();

            if (result is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Blog with id " + request.Blog.Id + " wasn't find."));

            BsonDocument doc = new BsonDocument("author_id", request.Blog.AuthorId)
                                                .Add("title", request.Blog.Title)
                                                .Add("content", request.Blog.Content);

            _mongoCollection.ReplaceOne(filter, doc);

            Blog.Blog blog = new()
            {
                AuthorId = request.Blog.AuthorId,
                Title = request.Blog.Title,
                Content = request.Blog.Content,
            };

            return new UpdateBlogResponse() { Blog = blog };
        }

        public override async Task<DeleteBlogResponse> DeleteBlog(DeleteBlogRequest request, ServerCallContext context)
        {
            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(request.BlogId));
            var result = _mongoCollection.Find(filter).FirstOrDefault();

            if (result is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Blog with id " + request.BlogId + " wasn't find."));

            var deleteResult = _mongoCollection.DeleteOne(filter);

            if (deleteResult.DeletedCount == 0)
                throw new RpcException(new Status(StatusCode.NotFound, "Blog with id " + request.BlogId + " wasn't find."));

            Blog.Blog blog = new()
            {
                Id = result.GetValue("_id").AsObjectId.ToString(),
                AuthorId = result.GetValue("author_id").AsString,
                Title = result.GetValue("title").AsString,
                Content = result.GetValue("content").AsString,
            };

            return new DeleteBlogResponse() { Blog = blog };
        }

        public override async Task ListBlog(ListBlogRequest request, IServerStreamWriter<ListBlogResponse> responseStream, ServerCallContext context)
        {
            var filter = new FilterDefinitionBuilder<BsonDocument>().Empty;

            var result = _mongoCollection.Find(filter);

            foreach (var item in result.ToList())
            {
                await responseStream.WriteAsync(new ListBlogResponse()
                {
                    Blog = new Blog.Blog()
                    {
                        Id = item.GetValue("_id").ToString(),
                        AuthorId = item.GetValue("author_id").AsString,
                        Title = item.GetValue("title").AsString,
                        Content = item.GetValue("content").AsString,
                    }
                });
            }
        }
    }
}