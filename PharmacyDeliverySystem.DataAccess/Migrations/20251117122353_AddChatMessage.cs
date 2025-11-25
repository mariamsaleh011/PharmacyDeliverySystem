using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyDeliverySystem.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__A4AE64B8BD45E9D6", x => x.CustomerID);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryRun",
                columns: table => new
                {
                    RunId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RiderId = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    EndAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Delivery__A259D4DDCE4D248D", x => x.RunId);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PayId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    METHOD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__EE8FCECF75FDE81D", x => x.PayId);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy",
                columns: table => new
                {
                    PharmId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LicenceNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pharmacy__16360C7F04B4C542", x => x.PharmId);
                });

            migrationBuilder.CreateTable(
                name: "QRConfirmation",
                columns: table => new
                {
                    QR_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    DeliveryRunId = table.Column<int>(type: "int", nullable: true),
                    EXP_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    ScannedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__QRConfir__7491345D2085C5B7", x => x.QR_Id);
                    table.ForeignKey(
                        name: "FK_QRConfirmation_DeliveryRun",
                        column: x => x.DeliveryRunId,
                        principalTable: "DeliveryRun",
                        principalColumn: "RunId");
                    table.ForeignKey(
                        name: "FK__QRConfirm__Custo__6C190EBB",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "Refund",
                columns: table => new
                {
                    RefId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PayId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Refund__2D2A2CF13D79525D", x => x.RefId);
                    table.ForeignKey(
                        name: "FK__Refund__PayId__619B8048",
                        column: x => x.PayId,
                        principalTable: "Payment",
                        principalColumn: "PayId");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    Quantity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PharmId = table.Column<int>(type: "int", nullable: true),
                    RunId = table.Column<int>(type: "int", nullable: true),
                    Pdf_Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Invoice_No = table.Column<int>(type: "int", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Payment_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Customer_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__C3905BAF842C6638", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Orders_Customer",
                        column: x => x.Customer_id,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK_Orders_Payment",
                        column: x => x.Payment_id,
                        principalTable: "Payment",
                        principalColumn: "PayId");
                    table.ForeignKey(
                        name: "FK__Orders__PharmId__4D94879B",
                        column: x => x.PharmId,
                        principalTable: "Pharmacy",
                        principalColumn: "PharmId");
                    table.ForeignKey(
                        name: "FK__Orders__RunId__4E88ABD4",
                        column: x => x.RunId,
                        principalTable: "DeliveryRun",
                        principalColumn: "RunId");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VAT_Rate = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    PharmId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PRODUCTS__62029590CEE37925", x => x.ProId);
                    table.ForeignKey(
                        name: "FK__PRODUCTS__PharmI__59063A47",
                        column: x => x.PharmId,
                        principalTable: "Pharmacy",
                        principalColumn: "PharmId");
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    chatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    Customer_id = table.Column<int>(type: "int", nullable: true),
                    Pharmacy_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Chat__826385AD5136740D", x => x.chatId);
                    table.ForeignKey(
                        name: "FK_Chat_Customer",
                        column: x => x.Customer_id,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK_Chat_Pharmacy",
                        column: x => x.Pharmacy_id,
                        principalTable: "Pharmacy",
                        principalColumn: "PharmId");
                    table.ForeignKey(
                        name: "FK__Chat__OrderID__5EBF139D",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Product_id = table.Column<int>(type: "int", nullable: false),
                    Order_id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => new { x.Product_id, x.Order_id });
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_Order_id",
                        column: x => x.Order_id,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    PreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Image = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    PharmId = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Prescrip__7024CEC96EDAC11B", x => x.PreId);
                    table.ForeignKey(
                        name: "FK__Prescript__Custo__5165187F",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                    table.ForeignKey(
                        name: "FK__Prescript__Order__534D60F1",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__Prescript__Pharm__52593CB8",
                        column: x => x.PharmId,
                        principalTable: "Pharmacy",
                        principalColumn: "PharmId");
                });

            migrationBuilder.CreateTable(
                name: "Return",
                columns: table => new
                {
                    ReturnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Returnn_F445E9A86A4CD3B7", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_Return_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    SenderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "chatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Customer_id",
                table: "Chat",
                column: "Customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_OrderID",
                table: "Chat",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Pharmacy_id",
                table: "Chat",
                column: "Pharmacy_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatId",
                table: "ChatMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__85FB4E385642804D",
                table: "Customer",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_Order_id",
                table: "OrderItem",
                column: "Order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Customer_id",
                table: "Orders",
                column: "Customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Payment_id",
                table: "Orders",
                column: "Payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PharmId",
                table: "Orders",
                column: "PharmId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RunId",
                table: "Orders",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_CustomerID",
                table: "Prescription",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_OrderID",
                table: "Prescription",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_PharmId",
                table: "Prescription",
                column: "PharmId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PharmId",
                table: "Products",
                column: "PharmId");

            migrationBuilder.CreateIndex(
                name: "IX_QRConfirmation_CustomerID",
                table: "QRConfirmation",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_QRConfirmation_DeliveryRunId",
                table: "QRConfirmation",
                column: "DeliveryRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_PayId",
                table: "Refund",
                column: "PayId");

            migrationBuilder.CreateIndex(
                name: "IX_Return_OrderID",
                table: "Return",
                column: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Prescription");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "QRConfirmation");

            migrationBuilder.DropTable(
                name: "Refund");

            migrationBuilder.DropTable(
                name: "Return");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Pharmacy");

            migrationBuilder.DropTable(
                name: "DeliveryRun");
        }
    }
}
