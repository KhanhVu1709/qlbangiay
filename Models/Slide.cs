namespace QlBanGiay.Models
{
	public class Slide
	{
		public int Id { get; set; }

		public string Anh { get; set; } = null!;

		public string Title { get; set; } = null!;

		public int ViTri { get; set; }

		public int IdTrangThai { get; set; }

		public virtual TrangThaiSlide IdTrangThaiNavigation { get; set; } = null!;
	}
}
