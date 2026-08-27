using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.API.Models;

namespace ParkingManagement.API.Data;

public partial class ParkingDbContext : DbContext
{
    public ParkingDbContext()
    {
    }

    public ParkingDbContext(DbContextOptions<ParkingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingExtension> BookingExtensions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<ParkingArea> ParkingAreas { get; set; }

    public virtual DbSet<ParkingLot> ParkingLots { get; set; }

    public virtual DbSet<ParkingSession> ParkingSessions { get; set; }

    public virtual DbSet<ParkingSlot> ParkingSlots { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }
    public virtual DbSet<ParkingLotAmenity> ParkingLotAmenities { get; set; }  
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ParkingManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Activity__5E5486486F6E4C94");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ActivityL__UserI__17036CC0");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED0698F041");

            entity.HasIndex(e => new { e.ParkingLotId, e.StartTime, e.EndTime }, "IX_Bookings_ParkingLot_Time");

            entity.HasIndex(e => e.Status, "IX_Bookings_Status");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ChoThanhToan");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 0)");

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Parkin__656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__UserId__6477ECF3");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Vehicl__66603565");
        });

        modelBuilder.Entity<BookingExtension>(entity =>
        {
            entity.HasKey(e => e.ExtensionId).HasName("PK__BookingE__5581AF2C01483A0A");

            entity.Property(e => e.Amount).HasColumnType("decimal(12, 0)");
            entity.Property(e => e.ExtendedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ExtendedHours).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingExtensions)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingEx__Booki__6E01572D");

            entity.HasOne(d => d.Payment).WithMany(p => p.BookingExtensions)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Extension_Payment");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F116DA2C0D5");

            entity.HasIndex(e => e.UserId, "UQ__Employee__1788CC4D7D78DE74").IsUnique();

            entity.Property(e => e.Shift).HasMaxLength(20);

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__Parki__4BAC3F29");

            entity.HasOne(d => d.User).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__UserI__4AB81AF0");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAD543078680");

            entity.HasIndex(e => new { e.UserId, e.ParkingLotId }, "UQ_Favorite").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__Parki__1332DBDC");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Favorites__UserI__123EB7A3");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70C82B61200");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.Images)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Images__ParkingL__1AD3FDA4");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12CA027E6D");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "IX_Notifications_User_Read");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.Type).HasMaxLength(30);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__05D8E0BE");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__Owners__819385B8186914A8");

            entity.HasIndex(e => e.UserId, "UQ__Owners__1788CC4D498C3934").IsUnique();

            entity.Property(e => e.BusinessName).HasMaxLength(150);
            entity.Property(e => e.TaxCode).HasMaxLength(30);

            entity.HasOne(d => d.User).WithOne(p => p.Owner)
                .HasForeignKey<Owner>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Owners__UserId__412EB0B6");
        });

        modelBuilder.Entity<ParkingArea>(entity =>
        {
            entity.HasKey(e => e.ParkingAreaId).HasName("PK__ParkingA__64FAA2AFCACB2E04");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.ParkingAreas)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingAr__Parki__5535A963");
        });

        modelBuilder.Entity<ParkingLot>(entity =>
        {
            entity.HasKey(e => e.ParkingLotId).HasName("PK__ParkingL__6F271E895BB12228");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Owner).WithMany(p => p.ParkingLots)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingLo__Owner__440B1D61");
        });

        modelBuilder.Entity<ParkingSession>(entity =>
        {
            entity.HasKey(e => e.ParkingSessionId).HasName("PK__ParkingS__AA4248327B9C1A25");

            entity.HasIndex(e => e.Status, "IX_ParkingSessions_Status");

            entity.Property(e => e.CheckInTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("DangGui");

            entity.HasOne(d => d.Booking).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__ParkingSe__Booki__71D1E811");

            entity.HasOne(d => d.EmployeeIdCheckInNavigation).WithMany(p => p.ParkingSessionEmployeeIdCheckInNavigations)
                .HasForeignKey(d => d.EmployeeIdCheckIn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingSe__Emplo__75A278F5");

            entity.HasOne(d => d.EmployeeIdCheckOutNavigation).WithMany(p => p.ParkingSessionEmployeeIdCheckOutNavigations)
                .HasForeignKey(d => d.EmployeeIdCheckOut)
                .HasConstraintName("FK__ParkingSe__Emplo__76969D2E");

            entity.HasOne(d => d.ParkingSlot).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.ParkingSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingSe__Parki__72C60C4A");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.ParkingSessions)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingSe__Vehic__73BA3083");
        });

        modelBuilder.Entity<ParkingSlot>(entity =>
        {
            entity.HasKey(e => e.ParkingSlotId).HasName("PK__ParkingS__5CFBE801D982DE9C");

            entity.HasIndex(e => e.Status, "IX_ParkingSlots_Status");

            entity.HasIndex(e => new { e.ParkingAreaId, e.SlotCode }, "UQ_Slot_Area_Code").IsUnique();

            entity.Property(e => e.SlotCode).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Trống");

            entity.HasOne(d => d.ParkingArea).WithMany(p => p.ParkingSlots)
                .HasForeignKey(d => d.ParkingAreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingSl__Parki__59063A47");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.ParkingSlots)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ParkingSl__Vehic__59FA5E80");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A3805519E84");

            entity.HasIndex(e => e.BookingId, "IX_Payments_Booking");

            entity.HasIndex(e => e.ParkingSessionId, "IX_Payments_Session");

            entity.Property(e => e.Amount).HasColumnType("decimal(12, 0)");
            entity.Property(e => e.PaidAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.Property(e => e.PaymentType).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("ThanhCong");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payments__Bookin__7C4F7684");

            entity.HasOne(d => d.ParkingSession).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ParkingSessionId)
                .HasConstraintName("FK__Payments__Parkin__7D439ABD");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.PriceId).HasName("PK__Prices__49575BAF23FA6C4D");

            entity.Property(e => e.EffectiveFrom).HasDefaultValueSql("(CONVERT([date],sysutcdatetime()))");
            entity.Property(e => e.PriceType).HasMaxLength(20);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(12, 0)");

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.Prices)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prices__ParkingL__5EBF139D");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Prices)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prices__VehicleT__5FB337D6");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE34A9138C");

            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.OwnerReply).HasMaxLength(500);

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Parking__0C85DE4D");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__0B91BA14");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A638FD1C3");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616068CBF9F6").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(30);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C7ED78604");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534769CD6B4").IsUnique();

            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3B75D760");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B549243AE9255");

            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.Nickname).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicles__UserId__5165187F");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicles__Vehicl__52593CB8");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.VehicleTypeId).HasName("PK__VehicleT__9F449643FC04F516");

            entity.HasIndex(e => e.TypeName, "UQ__VehicleT__D4E7DFA823F7F642").IsUnique();

            entity.Property(e => e.TypeName).HasMaxLength(50);
        });
        modelBuilder.Entity<ParkingLotAmenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId);
            entity.Property(e => e.Content).HasMaxLength(255).IsRequired();

            entity.HasOne(d => d.ParkingLot)
                .WithMany()
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
