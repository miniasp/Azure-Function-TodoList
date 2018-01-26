using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using TodoFunction.Entities;

namespace TodoFunction
{
    public static class TodoFunction
    {
        [FunctionName("GetAllTodo")]
        public static HttpResponseMessage/*Output*/ GetAllTodo(
            //Trigger
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]HttpRequestMessage req,
            // Input 
            [Table("Todo", Connection = "TableConnectionString")] IQueryable<Todo> todos,
            TraceWriter log)
        {
            log.Info("Get All Todo");

            var models = todos.Select(x => new
            {
                Key = x.RowKey,
                x.Task,
                x.Done
            });

            return req.CreateResponse(HttpStatusCode.OK, models);
        }

        [FunctionName("GetTodo")]
        public static HttpResponseMessage GetTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]HttpRequestMessage req,
            [Table("Todo", Connection = "TableConnectionString")] IQueryable<Todo> todos,
            string id,
            TraceWriter log)
        {
            log.Info($"Get Todo Id:{id}");

            var todo = todos
                .ToList()
                .Where(x => x.RowKey == id)
                .Select(x => new
                {
                    Key = x.RowKey,
                    x.Task,
                    x.Done
                })
                .FirstOrDefault();
            return req.CreateResponse(HttpStatusCode.OK, todo);
        }

        [FunctionName("AddTodo")]
        public static void AddTodo(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]string task,
             [Table("Todo", Connection = "TableConnectionString")] out Todo todo,
             [Queue("slack-queue")] out string slackQueue,
             TraceWriter log)
        {
            log.Info($"Add Todo:{task}");
            todo = new Todo
            {
                PartitionKey = "Http",
                Task = task,
                Done = false,
                RowKey = Guid.NewGuid().ToString()
            };

            slackQueue = $"·s¼W Todo {task}";

            var todoJson = JsonConvert.SerializeObject(new
            {
                todo.Task,
                todo.Done,
                Key = todo.RowKey
            });
        }

        [FunctionName("CompletedTodo")]
        public static HttpResponseMessage CompletedTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]HttpRequestMessage req,
            [Table("Todo" ,Connection = "TableConnectionString")] CloudTable todoTable,
            string id,
            TraceWriter log)
        {
            log.Info($"Update Todo:{id}");

            var result = todoTable.Execute(TableOperation.Retrieve<Todo>("Http", id));
            var todo = (Todo)result.Result;

            todo.Done = !todo.Done;

            todoTable.Execute(TableOperation.Replace(todo));

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        [FunctionName("DeleteTodo")]
        public static HttpResponseMessage DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")]HttpRequestMessage req,
            [Table("Todo", Connection = "TableConnectionString")] CloudTable todoTable,
            string id,
            TraceWriter log)
        {
            log.Info($"Delete Todo:{id}");

            var result = todoTable.Execute(TableOperation.Retrieve<Todo>("Http", id));
            var todo = (Todo)result.Result;

            todoTable.Execute(TableOperation.Delete(todo));

            return req.CreateResponse(HttpStatusCode.OK);
        }

        //[FunctionName("AddTodo")]
        //public static async Task<HttpResponseMessage> AddTodo(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Todo")]HttpRequestMessage req,
        //    [Table("Todo", Connection = "TableConnectionString")] CloudTable todoTable,
        //    TraceWriter log)
        //{
        //    string task =  await req.Content.ReadAsStringAsync();
        //    var todo = new Todo { PartitionKey = "Http", Text = text, RowKey = Guid.NewGuid().ToString() };
        //    todoTable.Execute(TableOperation.Insert(todo));

        //    return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
        //}
    }
}
