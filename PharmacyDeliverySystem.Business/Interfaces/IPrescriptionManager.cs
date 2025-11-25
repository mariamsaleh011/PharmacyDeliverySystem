using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.Business.Interfaces
{
	public interface IPrescriptionManager
	{
		IEnumerable<Prescription> GetAll();
		Prescription? GetById(int id);
		IEnumerable<Prescription> GetByCustomer(int customerId);
		IEnumerable<Prescription> GetByPharmacy(int pharmacyId);
		IEnumerable<Prescription> GetByOrder(int orderId);

		void Add(Prescription entity);
		void Update(Prescription entity);
		void SetStatus(int id, string status);  // Uploaded/UnderReview/Approved/Rejected/Fulfilled
		void Delete(int id);
	}
}
