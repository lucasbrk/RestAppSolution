using System.Collections.Generic;
using RestApp.Core.Domain.ItemCategorys;

namespace RestApp.Services.ItemCategorys
{
    /// <summary>
    /// ItemCategory service interface
    /// </summary>
    public partial interface IItemCategoryService
    {
        #region GETS

        /// <summary>
        /// Gets a ItemCategory
        /// </summary>
        /// <param name="itemCategoryId">ItemCategory identifier</param>
        /// <returns>ItemCategory</returns>
        ItemCategory GetItemCategoryById(int itemCategoryId);

        /// <summary>
        /// Gets a ItemCategory by Number
        /// </summary>
        /// <param name="name">ItemCategory Number</param>
        /// <returns>ItemCategory</returns>
        ItemCategory GetItemCategoryByName(string name);

        /// <summary>
        /// Gets all ItemCategorys
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>ItemCategory</returns>
        IList<ItemCategory> GetAllItemCategorys(bool showHidden = false);

        /// <summary>
        /// Avaible Name
        /// </summary>
        /// <param name="name, id">ItemCategory Name and Id</param>
        /// <returns>bool</returns>
        bool IsNameAvailable(string number, int id);

        #endregion

        #region Insert/Update/Delete

        /// <summary>
        /// Deletes a ItemCategory
        /// </summary>
        /// <param name="itemCategory">ItemCategory</param>
        void DeleteItemCategory(ItemCategory itemCategory);

        /// <summary>
        /// Inserts a ItemCategory
        /// </summary>
        /// <param name="itemCategory">ItemCategory</param>
        void InsertItemCategory(ItemCategory itemCategory);

        /// <summary>
        /// Updates the ItemCategory
        /// </summary>
        /// <param name="itemCategory">ItemCategory</param>
        void UpdateItemCategory(ItemCategory itemCategory);

        #endregion
    }
}
