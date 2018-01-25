using Microsoft.WindowsAzure.Storage.Table;

namespace TodoFunction.Entities
{
    public class Todo : TableEntity
    {
        public string Task { get; set; }
        public bool Done { get; set; }
    }
}
