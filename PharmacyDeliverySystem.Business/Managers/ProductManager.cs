using PharmacyDeliverySystem.Models;
using PharmacyDeliverySystem.DataAccess;


namespace PharmacyDeliverySystem.Business;

public class ProductManager : IProductManager
{
	private readonly PharmacyDeliveryContext context;
	public ProductManager(PharmacyDeliveryContext context)
	{
		context = context;
	}
	public IEnumerable<PRODUCT> GetAllProducts()
	{
		return this.context.PRODUCTs.ToList();
	}

	public PRODUCT? GetProductById(int id)
	{
		return this.context.PRODUCTs.Find(id);
	}

	public void AddProduct(PRODUCT product)
	{
		this.context.PRODUCTs.Add(product);
		this.context.SaveChanges();
	}

	public void UpdateProduct(PRODUCT product)
	{
		this.context.PRODUCTs.Update(product);
		this.context.SaveChanges();
	}

	public void DeleteProduct(int id)
	{
		this.context.PRODUCTs.Remove(this.context.PRODUCTs.Find(id));
		this.context.SaveChanges();
	}
}