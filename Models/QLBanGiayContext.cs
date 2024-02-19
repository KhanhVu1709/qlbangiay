using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QlBanGiay.Models;

namespace QlBanGiay.Models
{
    public partial class QLBanGiayContext : DbContext
    {
        public QLBanGiayContext()
        {
        }

        public QLBanGiayContext(DbContextOptions<QLBanGiayContext> options)
            : base(options)
        {
        }

		public virtual DbSet<Anh> Anhs { get; set; }

		public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

		public virtual DbSet<Hang> Hangs { get; set; }

		public virtual DbSet<HoaDon> HoaDons { get; set; }

		public virtual DbSet<KichThuoc> KichThuocs { get; set; }

		public virtual DbSet<LoaiUser> LoaiUsers { get; set; }

		public virtual DbSet<SanPham> SanPhams { get; set; }

		public virtual DbSet<SanPhamKichThuoc> SanPhamKichThuocs { get; set; }

		public virtual DbSet<Slide> Slides { get; set; }

		public virtual DbSet<TrangThai> TrangThais { get; set; }

		public virtual DbSet<TrangThaiSlide> TrangThaiSlides { get; set; }

		public virtual DbSet<User> Users { get; set; }

		public virtual DbSet<UserLoaiUser> UserLoaiUsers { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
			=> optionsBuilder.UseSqlServer("Data Source=DESKTOP-MP2EPV8\\SQLEXPRESS;Initial Catalog=QLBanGiay;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Anh>(entity =>
			{
				entity.ToTable("anh");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.IdSp).HasColumnName("idSP");
				entity.Property(e => e.TenFileAnh)
					.HasMaxLength(225)
					.HasColumnName("tenFileAnh");

				entity.HasOne(d => d.IdSpNavigation).WithMany(p => p.Anhs)
					.HasForeignKey(d => d.IdSp)
					.HasConstraintName("FK_anh_sanPham");
			});

			modelBuilder.Entity<ChiTietHoaDon>(entity =>
			{
				entity.ToTable("chiTietHoaDon");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.DonGia).HasColumnName("donGia");
				entity.Property(e => e.IdHoaDon).HasColumnName("idHoaDon");
				entity.Property(e => e.IdKichThuoc).HasColumnName("idKichThuoc");
				entity.Property(e => e.IdSp).HasColumnName("idSP");
				entity.Property(e => e.SlBan).HasColumnName("slBan");

				entity.HasOne(d => d.IdHoaDonNavigation).WithMany(p => p.ChiTietHoaDons)
					.HasForeignKey(d => d.IdHoaDon)
					.HasConstraintName("FK_chiTietHoaDon_hoaDon1");

				entity.HasOne(d => d.IdKichThuocNavigation).WithMany(p => p.ChiTietHoaDons)
					.HasForeignKey(d => d.IdKichThuoc)
					.HasConstraintName("FK_chiTietHoaDon_kichThuoc");

				entity.HasOne(d => d.IdSpNavigation).WithMany(p => p.ChiTietHoaDons)
					.HasForeignKey(d => d.IdSp)
					.HasConstraintName("FK_chiTietHoaDon_sanPham");
			});

			modelBuilder.Entity<Hang>(entity =>
			{
				entity.ToTable("hang");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.TenHang)
					.HasMaxLength(50)
					.HasColumnName("tenHang");
			});

			modelBuilder.Entity<HoaDon>(entity =>
			{
				entity.ToTable("hoaDon");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.IdTrangThai).HasColumnName("idTrangThai");
				entity.Property(e => e.IdUser).HasColumnName("idUser");
				entity.Property(e => e.NgayBan)
					.HasColumnType("datetime")
					.HasColumnName("ngayBan");

				entity.HasOne(d => d.IdTrangThaiNavigation).WithMany(p => p.HoaDons)
					.HasForeignKey(d => d.IdTrangThai)
					.HasConstraintName("FK_hoaDon_trangThai");

				entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.HoaDons)
					.HasForeignKey(d => d.IdUser)
					.HasConstraintName("FK_hoaDon_user");
			});

			modelBuilder.Entity<KichThuoc>(entity =>
			{
				entity.ToTable("kichThuoc");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.SoDo).HasColumnName("soDo");
			});

			modelBuilder.Entity<LoaiUser>(entity =>
			{
				entity.ToTable("loaiUser");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.TenLoaiUser)
					.HasMaxLength(50)
					.HasColumnName("tenLoaiUser");
			});

			modelBuilder.Entity<SanPham>(entity =>
			{
				entity.ToTable("sanPham");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.Anh)
					.HasMaxLength(225)
					.HasColumnName("anh");
				entity.Property(e => e.Gia).HasColumnName("gia");
				entity.Property(e => e.IdHang).HasColumnName("idHang");
				entity.Property(e => e.SoLuong).HasColumnName("soLuong");
				entity.Property(e => e.TenSp)
					.HasMaxLength(225)
					.HasColumnName("tenSP");
				entity.Property(e => e.ThoiGianBaoHanh).HasColumnName("thoiGianBaoHanh");

				entity.HasOne(d => d.IdHangNavigation).WithMany(p => p.SanPhams)
					.HasForeignKey(d => d.IdHang)
					.HasConstraintName("FK_sanPham_hang");
			});

			modelBuilder.Entity<SanPhamKichThuoc>(entity =>
			{
				entity.ToTable("sanPham_kichThuoc");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.IdKichThuoc).HasColumnName("idKichThuoc");
				entity.Property(e => e.IdSp).HasColumnName("idSP");
				entity.Property(e => e.SoLuong).HasColumnName("soLuong");

				entity.HasOne(d => d.IdKichThuocNavigation).WithMany(p => p.SanPhamKichThuocs)
					.HasForeignKey(d => d.IdKichThuoc)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_sanPham_kichThuoc_kichThuoc");

				entity.HasOne(d => d.IdSpNavigation).WithMany(p => p.SanPhamKichThuocs)
					.HasForeignKey(d => d.IdSp)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_sanPham_kichThuoc_sanPham");
			});

			modelBuilder.Entity<Slide>(entity =>
			{
				entity.ToTable("slides");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.Anh)
					.HasMaxLength(255)
					.HasColumnName("anh");
				entity.Property(e => e.IdTrangThai).HasColumnName("idTrangThai");
				entity.Property(e => e.Title)
					.HasMaxLength(255)
					.HasColumnName("title");
				entity.Property(e => e.ViTri).HasColumnName("viTri");

				entity.HasOne(d => d.IdTrangThaiNavigation).WithMany(p => p.Slides)
					.HasForeignKey(d => d.IdTrangThai)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_slides_trangThaiSlide");
			});

			modelBuilder.Entity<TrangThai>(entity =>
			{
				entity.ToTable("trangThai");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.TenTrangThai)
					.HasMaxLength(255)
					.HasColumnName("tenTrangThai");
			});

			modelBuilder.Entity<TrangThaiSlide>(entity =>
			{
				entity.ToTable("trangThaiSlide");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.TenTrangThai)
					.HasMaxLength(255)
					.HasColumnName("tenTrangThai");
			});

			modelBuilder.Entity<User>(entity =>
			{
				entity.ToTable("user");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.Anh)
					.HasMaxLength(255)
					.HasColumnName("anh");
				entity.Property(e => e.DiaChi)
					.HasMaxLength(225)
					.HasColumnName("diaChi");
				entity.Property(e => e.Email)
					.HasMaxLength(50)
					.HasColumnName("email");
				entity.Property(e => e.HoTen)
					.HasMaxLength(50)
					.HasColumnName("hoTen");
				entity.Property(e => e.MatKhau)
					.HasMaxLength(255)
					.HasColumnName("matKhau");
				entity.Property(e => e.Sdt)
					.HasMaxLength(10)
					.HasColumnName("sdt");
				entity.Property(e => e.TaiKhoan)
					.HasMaxLength(30)
					.HasColumnName("taiKhoan");
				entity.Property(e => e.TrangThai).HasColumnName("trangThai");
			});

			modelBuilder.Entity<UserLoaiUser>(entity =>
			{
				entity.ToTable("user_loaiUser");

				entity.Property(e => e.Id).HasColumnName("id");
				entity.Property(e => e.IdLoaiUser).HasColumnName("idLoaiUser");
				entity.Property(e => e.IdUser).HasColumnName("idUser");

				entity.HasOne(d => d.IdLoaiUserNavigation).WithMany(p => p.UserLoaiUsers)
					.HasForeignKey(d => d.IdLoaiUser)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_user_loaiUser_loaiUser");

				entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserLoaiUsers)
					.HasForeignKey(d => d.IdUser)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_user_loaiUser_user");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
