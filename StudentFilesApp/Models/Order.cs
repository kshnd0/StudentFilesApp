using System;
using System.Collections.Generic;

namespace StudentFilesApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderTypeId { get; set; }
        public OrderType OrderType { get; set; }
        public string Comment { get; set; }
        public ICollection<OrderStudent> OrderStudents { get; set; } = new List<OrderStudent>();
    }

    public class OrderType
    {
        public int OrderTypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class OrderStudent
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}