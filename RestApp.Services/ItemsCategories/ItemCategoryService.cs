using System;
using System.Collections.Generic;
using System.Linq;
using RestApp.Core.Data;
using RestApp.Core.Domain.ItemCategorys;

namespace RestApp.Services.ItemCategorys
{
    public class ItemCategoryService : IItemCategoryService
    {
        #region Fields

        private readonly IRepository<ItemCategory> gItemCategoryRepository;

        #endregion

        #region Ctor

        public ItemCategoryService(IRepository<ItemCategory> itemCategoryRepository)
        {
            this.gItemCategoryRepository = itemCategoryRepository;
        }

        #endregion

        #region GETS
       
        public virtual ItemCategory GetItemCategoryById(int itemCategoryId)
        {
            if (itemCategoryId == 0)
                return null;

            return gItemCategoryRepository.GetById(itemCategoryId);
        }
        
        public virtual ItemCategory GetItemCategoryByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            var query = gItemCategoryRepository.Table;
            query = query.Where(st => st.Name == name);
            query = query.OrderByDescending(t => t.Id);

            var itemCategory = query.FirstOrDefault();
            return itemCategory;
        }

        public virtual IList<ItemCategory> GetAllItemCategorys(bool showHidden = false)
        {
            var query = gItemCategoryRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }
            query = query.OrderByDescending(t => t.Id);

            var itemCategorys = query.ToList();
            return itemCategorys;
        }

        public bool IsNameAvailable(string number, int id)
        {
            if (String.IsNullOrWhiteSpace(number))
                throw new Exception("Invalid Number");

            var query = gItemCategoryRepository.Table
                        .Where(st => st.Name == number &&
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        #endregion

        #region Insert/Update/Delete

        public virtual void DeleteItemCategory(ItemCategory itemCategory)
        {
            if (itemCategory == null)
                throw new ArgumentNullException("itemCategory");

            gItemCategoryRepository.Delete(itemCategory);
        }  

        public virtual void InsertItemCategory(ItemCategory itemCategory)
        {
            if (itemCategory == null)
                throw new ArgumentNullException("itemCategory");

            gItemCategoryRepository.Insert(itemCategory);
        }

        public virtual void UpdateItemCategory(ItemCategory itemCategory)
        {
            if (itemCategory == null)
                throw new ArgumentNullException("itemCategory");

            gItemCategoryRepository.Update(itemCategory);
        }

        #endregion        
    }
}