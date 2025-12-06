using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Models;
using System;
using System.Collections.Generic;

namespace PharmacyDeliverySystem.DataAccess
{
    public partial class PharmacyDeliveryContext : DbContext
    {
        public PharmacyDeliveryContext(DbContextOptions<PharmacyDeliveryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Chat> Chats { get; set; }

        // ✅ واحد بس لـ ChatMessages
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DeliveryRun> DeliveryRuns { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Pharmacy> Pharmacies { get; set; }
        public virtual DbSet<Prescription> Prescriptions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<QrConfirmation> QrConfirmations { get; set; }
        public virtual DbSet<Refund> Refunds { get; set; }

        // ✅ استخدمنا Return مش Returnn
        public virtual DbSet<Return> Returns { get; set; }
        public virtual DbSet<PharmacyAdmin> PharmacyAdmins { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.ChatId).HasName("PK__Chat__826385AD5136740D");

                entity.ToTable("Chat");

                entity.Property(e => e.ChatId).HasColumnName("chatId");
                entity.Property(e => e.Channel).HasMaxLength(20);
                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.PharmacyId).HasColumnName("Pharmacy_id");
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Customer).WithMany(p => p.Chats)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Chat_Customer");

                entity.HasOne(d => d.Order).WithMany(p => p.Chats)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Chat__OrderID__5EBF139D");

                entity.HasOne(d => d.Pharmacy).WithMany(p => p.Chats)
                    .HasForeignKey(d => d.PharmacyId)
                    .HasConstraintName("FK_Chat_Pharmacy");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8BD45E9D6");

                entity.ToTable("Customer");

                entity.HasIndex(e => e.PhoneNumber, "UQ__Customer__85FB4E385642804D").IsUnique();

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            });

            modelBuilder.Entity<DeliveryRun>(entity =>
            {
                entity.HasKey(e => e.RunId).HasName("PK__Delivery__A259D4DDCE4D248D");

                entity.ToTable("DeliveryRun");

                entity.Property(e => e.EndAt).HasColumnType("smalldatetime");
                entity.Property(e => e.StartAt).HasColumnType("smalldatetime");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF842C6638");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.CustomerId).HasColumnName("Customer_id");
                entity.Property(e => e.InvoiceNo).HasColumnName("Invoice_No");
                entity.Property(e => e.PaymentId)
                    .HasMaxLength(255)
                    .HasColumnName("Payment_id");
                entity.Property(e => e.PdfUrl)
                    .HasMaxLength(255)
                    .HasColumnName("Pdf_Url");
                entity.Property(e => e.Price).HasColumnType("decimal(20, 0)");
                entity.Property(e => e.Quantity).HasMaxLength(20);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Orders_Customer");

                entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Orders_Payment");

                entity.HasOne(d => d.Pharm).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PharmId)
                    .HasConstraintName("FK__Orders__PharmId__4D94879B");

                entity.HasOne(d => d.Run).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.RunId)
                    .HasConstraintName("FK__Orders__RunId__4E88ABD4");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.OrderId });

                entity.ToTable("OrderItem");

                entity.Property(e => e.ProductId).HasColumnName("Product_id");
                entity.Property(e => e.OrderId).HasColumnName("Order_id");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PayId).HasName("PK__Payment__EE8FCECF75FDE81D");

                entity.ToTable("Payment");

                entity.Property(e => e.PayId).HasMaxLength(255);
                entity.Property(e => e.Method)
                    .HasMaxLength(20)
                    .HasColumnName("METHOD");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<Pharmacy>(entity =>
            {
                entity.HasKey(e => e.PharmId).HasName("PK__Pharmacy__16360C7F04B4C542");

                entity.ToTable("Pharmacy");

                entity.Property(e => e.LicenceNo).HasMaxLength(20);
                entity.Property(e => e.Name).HasMaxLength(30);
                entity.Property(e => e.TaxId).HasMaxLength(20);
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.PreId).HasName("PK__Prescrip__7024CEC96EDAC11B");

                entity.ToTable("Prescription");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Customer).WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Prescript__Custo__5165187F");

                entity.HasOne(d => d.Order).WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Prescript__Order__534D60F1");

                entity.HasOne(d => d.Pharm).WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.PharmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Prescript__Pharm__52593CB8");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProId).HasName("PK__PRODUCTS__62029590CEE37925");

                entity.Property(e => e.Barcode).HasMaxLength(40);
                entity.Property(e => e.Brand).HasMaxLength(20);
                entity.Property(e => e.Dosage).HasMaxLength(40);
                entity.Property(e => e.Name).HasMaxLength(20);
                entity.Property(e => e.VatRate)
                    .HasMaxLength(30)
                    .HasColumnName("VAT_Rate");

                entity.HasOne(d => d.Pharm).WithMany(p => p.Products)
                    .HasForeignKey(d => d.PharmId)
                    .HasConstraintName("FK__PRODUCTS__PharmI__59063A47");
            });

            modelBuilder.Entity<QrConfirmation>(entity =>
            {
                entity.HasKey(e => e.QR_Id).HasName("PK__QRConfir__7491345D2085C5B7");

                entity.ToTable("QRConfirmation");

                entity.Property(e => e.QR_Id).HasColumnName("QR_Id");
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.EXP_date)
                    .HasColumnType("datetime")
                    .HasColumnName("EXP_date");
                entity.Property(e => e.ScannedBy).HasMaxLength(20);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.QrConfirmations)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__QRConfirm__Custo__6C190EBB");

                entity.HasOne(d => d.DeliveryRun)
                    .WithMany(p => p.QrConfirmations)
                    .HasForeignKey(d => d.DeliveryRunId)
                    .HasConstraintName("FK_QRConfirmation_DeliveryRun");
            });

            modelBuilder.Entity<Refund>(entity =>
            {
                entity.HasKey(e => e.RefId).HasName("PK__Refund__2D2A2CF13D79525D");

                entity.ToTable("Refund");

                entity.Property(e => e.PayId).HasMaxLength(255);
                entity.Property(e => e.Reason).HasMaxLength(200);

                entity.HasOne(d => d.Pay).WithMany(p => p.Refunds)
                    .HasForeignKey(d => d.PayId)
                    .HasConstraintName("FK__Refund__PayId__619B8048");
            });

            // ✅ Mapping الصحيح لـ Return
            modelBuilder.Entity<Return>(entity =>
            {
                entity.HasKey(e => e.ReturnId)
                      .HasName("PK_Returnn_F445E9A86A4CD3B7"); // اسم الكونسترينت مش فارق للكود

                entity.ToTable("Return");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.Reason).HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(r => r.Order)
                      .WithMany(o => o.Returns)  // لازم تبقي موجودة في Order
                      .HasForeignKey(r => r.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
