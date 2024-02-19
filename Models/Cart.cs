using JetBrains.Annotations;
using Newtonsoft.Json;
using QlBanGiay.Models;

namespace QlBanGiay.Models
{
	public class Cart
	{
		public static readonly QLBanGiayContext db = new QLBanGiayContext();
		public static T GetObjectFromJson<T>(ISession session, string key)
		{
			var value = session.GetString(key);
			return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
		}
		// 
		public static List<Item> GetCart(ISession session)
		{
			List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
			return cart;
		}
		// 
		public static void AddItem(ISession session, int idSP, int idKT, int quantity)
		{
			if (Cart.GetObjectFromJson<List<Item>>(session, "cart") == null)
			{
				List<Item> cart = new List<Item>();
				SanPham itemSP = db.SanPhams.Where(tbl => tbl.Id == idSP).FirstOrDefault();
				KichThuoc itemKT = db.KichThuocs.Where(ikt => ikt.Id == idKT).FirstOrDefault();

				cart.Add(new Item { sanPham = itemSP, kichThuoc = itemKT, soLuong = quantity });
				session.SetString("cart", JsonConvert.SerializeObject(cart));
			}
			else
			{
				List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");

				int index = Cart.isExist(session, idSP, idKT);
				if (index != -1)
				{
					cart[index].soLuong += quantity;
				}
				else
				{
					SanPham itemSP = db.SanPhams.Where(tbl => tbl.Id == idSP).FirstOrDefault();
					KichThuoc itemKT = db.KichThuocs.Where(ikt => ikt.Id == idKT).FirstOrDefault();

					cart.Add(new Item { sanPham = itemSP, kichThuoc = itemKT, soLuong = quantity });
				}
				session.SetString("cart", JsonConvert.SerializeObject(cart));
			}
		}
		// tru so luong di 1
		public static void UnAddItem(ISession session, int idSP, int idKT)
		{
			if (Cart.GetObjectFromJson<List<Item>>(session, "cart") == null)
			{
				List<Item> cart = new List<Item>();
				SanPham itemSP = db.SanPhams.Where(tbl => tbl.Id == idSP).FirstOrDefault();
				KichThuoc itemKT = db.KichThuocs.Where(ikt => ikt.Id == idKT).FirstOrDefault();

				cart.Add(new Item { sanPham = itemSP, kichThuoc = itemKT, soLuong = -1 });
				session.SetString("cart", JsonConvert.SerializeObject(cart));
			}
			else
			{
				List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");

				int index = Cart.isExist(session, idSP, idKT);
				if (index != -1)
				{
					cart[index].soLuong--;
				}
				else
				{
					SanPham itemSP = db.SanPhams.Where(tbl => tbl.Id == idSP).FirstOrDefault();
					KichThuoc itemKT = db.KichThuocs.Where(ikt => ikt.Id == idKT).FirstOrDefault();

					cart.Add(new Item { sanPham = itemSP, kichThuoc = itemKT, soLuong = -1 });
				}
				session.SetString("cart", JsonConvert.SerializeObject(cart));
			}
		}
		//
		public static void RemoveLine(ISession session, int idSP, int idKT)
		{
			List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
			int index = isExist(session, idSP, idKT);
			cart.RemoveAt(index);
			session.SetString("cart", JsonConvert.SerializeObject(cart));
		}
		//
		public static void CartDestroy(ISession session)
		{
			List<Item> cart = new List<Item>();
			session.SetString("cart", JsonConvert.SerializeObject(cart));
		}
		// 
		public static void CartUpdate(ISession session, int id, int soLuong)
		{
			List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
			//---
			for (int i = 0; i < cart.Count; i++)
			{
				if (cart[i].sanPham.Id == id)
				{
					cart[i].soLuong = soLuong;
				}
			}
			//---
			session.SetString("cart", JsonConvert.SerializeObject(cart));
		}
		//
		public static double CartTotal(ISession session)
		{
			List<Item> items_cart = Cart.GetCart(session);
			if (items_cart != null)
			{
				double total = 0;
				foreach (var item in items_cart)
				{
					total += Convert.ToDouble(item.soLuong * item.sanPham.Gia);
				}
				return total;
			}
			else
				return 0;
		}
		// kiểm tra sản phẩm theo id này đã tồn tại trong giỏ hàng hay chưa
		public static int isExist(ISession session, int idSP, int idKT)
		{
			List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
			for (int i = 0; i < cart.Count; i++)
			{
				if (cart[i].sanPham.Id == idSP && cart[i].kichThuoc.Id == idKT)
				{
					return i;
				}
			}
			return -1;
		}
		//
		public static int CartSoLuong(ISession session)
		{
			List<Item> items_cart = Cart.GetCart(session);
			if (items_cart != null)
			{
				return items_cart.Count;
			}
			else
				return 0;
		}
		// 
		//public static void CartCheckout(ISession session, int id_user)
		//{
		//	List<Item> cart = Cart.GetCart(session);

		//	HoaDon hoaDon = new HoaDon();
		//	hoaDon.IdUser = id_user;
		//	hoaDon.NgayBan = DateTime.Now;
			
		//}
	}
}
