namespace InventoryData.IRepo
{
    public interface IUnitOfWork
    {
        public IProductRepo Product { get; }

        public ISupplierRepo Supplier { get; }

        public IAppUserRepo AppUser { get; }


        Task SaveAsync();
    }
}
