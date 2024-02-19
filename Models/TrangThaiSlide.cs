namespace QlBanGiay.Models
{
	public class TrangThaiSlide
	{
		public int Id { get; set; }

		public string TenTrangThai { get; set; } = null!;

		public virtual ICollection<Slide> Slides { get; set; } = new List<Slide>();
	}
}
