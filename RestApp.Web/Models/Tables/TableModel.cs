using System;
using System.Linq;
using RestApp.Web.Framework;

namespace RestApp.Web.Models.Tables
{
    public class TableModel
    {
        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.Table.Number")]
        public string Number { get; set; }

        [RestAppResourceDisplayName("Model.Table.Seat")]
        public string Seat { get; set; }

        [RestAppResourceDisplayName("Model.Common.Enabled")]
        public bool Enabled { get; set; }
    }
}