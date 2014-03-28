using System;
using System.Collections.Generic;
using System.Linq;
using RestApp.Core.Data;
using RestApp.Core.Domain.Tables;

namespace RestApp.Services.Tables
{
    public class TableService : ITableService
    {
        #region Fields

        private readonly IRepository<Table> gTableRepository;

        #endregion

        #region Ctor

        public TableService(IRepository<Table> tableRepository)
        {
            this.gTableRepository = tableRepository;
        }

        #endregion

        #region GETS
       
        public virtual Table GetTableById(int tableId)
        {
            if (tableId == 0)
                return null;

            return gTableRepository.GetById(tableId);
        }
        
        public virtual Table GetTableByNumber(string number)
        {
            if (String.IsNullOrWhiteSpace(number))
                return null;

            var query = gTableRepository.Table;
            query = query.Where(st => st.Number == number);
            query = query.OrderByDescending(t => t.Id);

            var table = query.FirstOrDefault();
            return table;
        }

        public virtual IList<Table> GetAllTables(bool showHidden = false)
        {
            var query = gTableRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }
            query = query.OrderByDescending(t => t.Id);

            var tables = query.ToList();
            return tables;
        }

        public bool IsNumberAvailable(string number, int id)
        {
            if (String.IsNullOrWhiteSpace(number))
                throw new Exception("Invalid Number");

            var query = gTableRepository.Table
                        .Where(st => st.Number == number &&
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        #endregion

        #region Insert/Update/Delete

        public virtual void DeleteTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            gTableRepository.Delete(table);
        }  

        public virtual void InsertTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            gTableRepository.Insert(table);
        }

        public virtual void UpdateTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            gTableRepository.Update(table);
        }

        #endregion        
    }
}