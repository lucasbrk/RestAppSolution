using System.Collections.Generic;
using RestApp.Core.Domain.Tables;

namespace RestApp.Services.Tables
{
    /// <summary>
    /// Table service interface
    /// </summary>
    public partial interface ITableService
    {
        #region GETS

        /// <summary>
        /// Gets a Table
        /// </summary>
        /// <param name="tableId">Table identifier</param>
        /// <returns>Table</returns>
        Table GetTableById(int tableId);

        /// <summary>
        /// Gets a Table by Number
        /// </summary>
        /// <param name="number">Table Number</param>
        /// <returns>Table</returns>
        Table GetTableByNumber(string number);

        /// <summary>
        /// Gets all Tables
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Table</returns>
        IList<Table> GetAllTables(bool showHidden = false);

        /// <summary>
        /// Avaible Number
        /// </summary>
        /// <param name="number, id">Table Name and Id</param>
        /// <returns>bool</returns>
        bool IsNumberAvailable(string number, int id);

        #endregion

        #region Insert/Update/Delete

        /// <summary>
        /// Deletes a Table
        /// </summary>
        /// <param name="table">Table</param>
        void DeleteTable(Table table);

        /// <summary>
        /// Inserts a Table
        /// </summary>
        /// <param name="table">Table</param>
        void InsertTable(Table table);

        /// <summary>
        /// Updates the Table
        /// </summary>
        /// <param name="table">Table</param>
        void UpdateTable(Table table);

        #endregion
    }
}
