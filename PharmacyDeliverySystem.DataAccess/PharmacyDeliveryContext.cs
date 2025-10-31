using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Models;

namespace PharmacyDeliverySystem.DataAccess;

public partial class PharmacyDeliveryContext : DbContext
{
    public PharmacyDeliveryContext(DbContextOptions<PharmacyDeliveryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerChat> CustomerChats { get; set; }

    public virtual DbSet<DeliveryRun> DeliveryRuns { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<Order_Invoice> Order_Invoices { get; set; }

    public virtual DbSet<PRODUCT> PRODUCTs { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Pharmacy> Pharmacies { get; set; }

    public virtual DbSet<PharmacyChat> PharmacyChats { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<QRConfirmation> QRConfirmations { get; set; }

    public virtual DbSet<QR_Dell> QR_Dells { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Returnn> Returnns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.chatId).HasName("PK__Chat__826385AD5136740D");

            entity.ToTable("Chat");

            entity.Property(e => e.Channel).HasMaxLength(20);
            entity.Property(e => e.Statuss).HasMaxLength(20);

            entity.HasOne(d => d.Order).WithMany(p => p.Chats)
                .HasForeignKey(d => d.OrderID)
                .HasConstraintName("FK__Chat__OrderID__5EBF139D");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerID).HasName("PK__Customer__A4AE64B8BD45E9D6");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Customer__85FB4E385642804D").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);

            entity.HasMany(d => d.Orders).WithMany(p => p.Customers)
                .UsingEntity<Dictionary<string, object>>(
                    "CustomerOrderChat",
                    r => r.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CustomerO__Order__04E4BC85"),
                    l => l.HasOne<Customer>().WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CustomerO__Custo__03F0984C"),
                    j =>
                    {
                        j.HasKey("CustomerID", "OrderID").HasName("PK__Customer__48976102506D6F94");
                        j.ToTable("CustomerOrderChat");
                    });
        });

        modelBuilder.Entity<CustomerChat>(entity =>
        {
            entity.HasKey(e => e.CustomerChatId).HasName("PK__Customer__95F291DDB4080032");

            entity.ToTable("CustomerChat");

            entity.HasOne(d => d.Chat).WithMany(p => p.CustomerChats)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerC__ChatI__00200768");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerChats)
                .HasForeignKey(d => d.CustomerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomerC__Custo__01142BA1");
        });

        modelBuilder.Entity<DeliveryRun>(entity =>
        {
            entity.HasKey(e => e.RunId).HasName("PK__Delivery__A259D4DDCE4D248D");

            entity.ToTable("DeliveryRun");

            entity.Property(e => e.EndAt).HasColumnType("smalldatetime");
            entity.Property(e => e.StartAt).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvId).HasName("PK__Invoice__9DC82C6A920C0A0B");

            entity.ToTable("Invoice");

            entity.HasOne(d => d.Pay).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PayId)
                .HasConstraintName("FK__Invoice__PayId__5629CD9C");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderID).HasName("PK__Orders__C3905BAF842C6638");

            entity.Property(e => e.Price).HasColumnType("decimal(20, 0)");
            entity.Property(e => e.Quantity).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Pharm).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PharmId)
                .HasConstraintName("FK__Orders__PharmId__4D94879B");

            entity.HasOne(d => d.Run).WithMany(p => p.Orders)
                .HasForeignKey(d => d.RunId)
                .HasConstraintName("FK__Orders__RunId__4E88ABD4");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OrderItem");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TimeId).HasColumnType("smalldatetime");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__5BE2A6F2");

            entity.HasOne(d => d.Pro).WithMany()
                .HasForeignKey(d => d.ProId)
                .HasConstraintName("FK__OrderItem__ProId__5AEE82B9");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.ProId).HasName("PK__OrderPro__620295909E54B4D2");

            entity.Property(e => e.ProId).ValueGeneratedNever();

            entity.HasOne(d => d.Order).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.OrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderProd__Order__0F624AF8");

            entity.HasOne(d => d.Pro).WithOne(p => p.OrderProduct)
                .HasForeignKey<OrderProduct>(d => d.ProId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderProd__ProId__0E6E26BF");
        });

        modelBuilder.Entity<Order_Invoice>(entity =>
        {
            entity.HasKey(e => e.OrderID).HasName("PK__Order_In__C3905BAFEC66B92E");

            entity.ToTable("Order_Invoice");

            entity.HasIndex(e => e.InvId, "UQ__Order_In__9DC82C6B760EB7A2").IsUnique();

            entity.Property(e => e.OrderID).ValueGeneratedNever();

            entity.HasOne(d => d.Inv).WithOne(p => p.Order_Invoice)
                .HasForeignKey<Order_Invoice>(d => d.InvId)
                .HasConstraintName("FK__Order_Inv__InvId__693CA210");

            entity.HasOne(d => d.Order).WithOne(p => p.Order_Invoice)
                .HasForeignKey<Order_Invoice>(d => d.OrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Inv__Order__68487DD7");
        });

        modelBuilder.Entity<PRODUCT>(entity =>
        {
            entity.HasKey(e => e.ProId).HasName("PK__PRODUCTS__62029590CEE37925");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.Barcode).HasMaxLength(40);
            entity.Property(e => e.Brand).HasMaxLength(20);
            entity.Property(e => e.Dosage).HasMaxLength(40);
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.VAC_Rate).HasMaxLength(30);

            entity.HasOne(d => d.Pharm).WithMany(p => p.PRODUCTs)
                .HasForeignKey(d => d.PharmId)
                .HasConstraintName("FK__PRODUCTS__PharmI__59063A47");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PayId).HasName("PK__Payment__EE8FCECF75FDE81D");

            entity.ToTable("Payment");

            entity.Property(e => e.METHOD).HasMaxLength(20);
            entity.Property(e => e.status).HasMaxLength(20);

            entity.HasMany(d => d.Invs).WithMany(p => p.Pays)
                .UsingEntity<Dictionary<string, object>>(
                    "InvoicePay",
                    r => r.HasOne<Invoice>().WithMany()
                        .HasForeignKey("InvId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__InvoicePa__InvId__160F4887"),
                    l => l.HasOne<Payment>().WithMany()
                        .HasForeignKey("PayId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__InvoicePa__PayId__151B244E"),
                    j =>
                    {
                        j.HasKey("PayId", "InvId").HasName("PK__InvoiceP__57534C096F285CD1");
                        j.ToTable("InvoicePay");
                    });
        });

        modelBuilder.Entity<Pharmacy>(entity =>
        {
            entity.HasKey(e => e.PharmId).HasName("PK__Pharmacy__16360C7F04B4C542");

            entity.ToTable("Pharmacy");

            entity.Property(e => e.LicenceNo).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.TaxId).HasMaxLength(20);
        });

        modelBuilder.Entity<PharmacyChat>(entity =>
        {
            entity.HasKey(e => e.PharmacyChatId).HasName("PK__Pharmacy__1CEB1D34740E41BC");

            entity.ToTable("PharmacyChat");

            entity.HasOne(d => d.Chat).WithMany(p => p.PharmacyChats)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PharmacyC__ChatI__6EF57B66");

            entity.HasOne(d => d.Pharm).WithMany(p => p.PharmacyChats)
                .HasForeignKey(d => d.PharmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PharmacyC__Pharm__6FE99F9F");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PreId).HasName("PK__Prescrip__7024CEC96EDAC11B");

            entity.ToTable("Prescription");

            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Customer).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.CustomerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Custo__5165187F");

            entity.HasOne(d => d.Order).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.OrderID)
                .HasConstraintName("FK__Prescript__Order__534D60F1");

            entity.HasOne(d => d.Pharm).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PharmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Prescript__Pharm__52593CB8");
        });

        modelBuilder.Entity<QRConfirmation>(entity =>
        {
            entity.HasKey(e => e.QR_Id).HasName("PK__QRConfir__7491345D2085C5B7");

            entity.ToTable("QRConfirmation");

            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.EXP_date).HasColumnType("datetime");
            entity.Property(e => e.ScannedBy).HasMaxLength(20);

            entity.HasOne(d => d.Customer).WithMany(p => p.QRConfirmations)
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK__QRConfirm__Custo__6C190EBB");
        });

        modelBuilder.Entity<QR_Dell>(entity =>
        {
            entity.HasKey(e => e.QR_id).HasName("PK__QR_Dell__74923045CE1C7948");

            entity.ToTable("QR_Dell");

            entity.Property(e => e.QR_id).ValueGeneratedNever();

            entity.HasOne(d => d.QR).WithOne(p => p.QR_Dell)
                .HasForeignKey<QR_Dell>(d => d.QR_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QR_Dell__QR_id__0A9D95DB");

            entity.HasOne(d => d.Run).WithMany(p => p.QR_Dells)
                .HasForeignKey(d => d.RunId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QR_Dell__RunId__0B91BA14");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefId).HasName("PK__Refund__2D2A2CF13D79525D");

            entity.ToTable("Refund");

            entity.Property(e => e.Reason).HasMaxLength(200);

            entity.HasOne(d => d.Pay).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PayId)
                .HasConstraintName("FK__Refund__PayId__619B8048");
        });

        modelBuilder.Entity<Returnn>(entity =>
        {
            entity.HasKey(e => e.ReturnId).HasName("PK__Returnn__F445E9A86A4CD3B7");

            entity.ToTable("Returnn");

            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.Order).WithMany(p => p.Returnns)
                .HasForeignKey(d => d.OrderID)
                .HasConstraintName("FK__Returnn__OrderID__6477ECF3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
