using System;

namespace RestApp.Core.Domain.Tables
{
    public class Table : BaseEntity
    {
        public string Number { get; set; }

        public string Seat { get; set; }

        public bool Enabled { get; set; }
    }
}
