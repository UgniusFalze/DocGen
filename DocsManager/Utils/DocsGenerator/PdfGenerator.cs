using System.Globalization;
using DocsManager.Models.Dto;
using DocsManager.Services.IntegerToWordsConverter;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DocsManager.Utils.DocsGenerator;

public class PdfGenerator : IPdfGenerator
{
    public byte[] GenerateInvoicePdf(InvoiceDto invoiceDto)
    {
        var document = CreateDocument(invoiceDto);
        return document.GeneratePdf();
    }

    private IDocument CreateDocument(InvoiceDto invoiceDto)
    {
        return Document.Create(document =>
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(8).FontFamily("Arial"));
                page.Header()
                    .Text(x =>
                    {
                        x.DefaultTextStyle(style => style.FontSize(10));
                        x.AlignCenter();
                        x.Line("SĄSKAITA FAKTŪRA");
                        x.Span("Serija ");
                        x.Span("AV").Bold();
                        x.Line(" Nr. " + invoiceDto.SeriesNumber);
                    });
                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(40);
                        column.Item().Text(text =>
                        {
                            text.Span("Išrašymo data: ").Bold();
                            text.Line(invoiceDto.Date);
                            text.AlignRight();
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Column(sellerColumn =>
                            {
                                sellerColumn
                                    .Item()
                                    .Text("PARDAVĖJAS:")
                                    .Bold();
                                sellerColumn
                                    .Item()
                                    .Text(invoiceDto.Name)
                                    .Bold();
                                sellerColumn
                                    .Item()
                                    .Text("Adresas: " + invoiceDto.Address);
                                sellerColumn
                                    .Item()
                                    .Text("Asmens kodas: " + invoiceDto.PersonalId);
                                sellerColumn
                                    .Item()
                                    .Text("Ne PVM mokėtojas");
                                sellerColumn
                                    .Item()
                                    .Text("Veikiantis pagal individualios veiklos vykdymo pažymą");
                                sellerColumn
                                    .Item()
                                    .Text("Nr. " + invoiceDto.FreelanceWorkId);
                                sellerColumn
                                    .Item()
                                    .Text("A. s.: " + invoiceDto.BankNumber);
                                sellerColumn
                                    .Item()
                                    .Text(invoiceDto.BankName);
                            });
                            row.RelativeItem().AlignRight().Column(buyerColumn =>
                            {
                                buyerColumn
                                    .Item()
                                    .Text("PIRKĖJAS:")
                                    .Bold();
                                buyerColumn
                                    .Item()
                                    .Text(invoiceDto.BuyerName)
                                    .Bold();
                                buyerColumn
                                    .Item()
                                    .Text("Adresas: " + invoiceDto.BuyerAddress);
                                buyerColumn
                                    .Item()
                                    .Text("Įmonės kodas: " + invoiceDto.BuyerCode);
                                if (invoiceDto.VatCode != null)
                                {
                                    buyerColumn
                                        .Item()
                                        .Text("Įmonės PVM kodas: " + invoiceDto.VatCode);
                                }
                            });
                        });
                        column.Item().BorderTop(2).BorderBottom(1).Table(table =>
                        {

                            IContainer DefaultCellStyle(IContainer container)
                            {
                                return container
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }

                            IContainer CellStyle(IContainer container) => DefaultCellStyle(container.BorderTop(1));
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(180);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(DefaultCellStyle).Text("Eil.\nNr.").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Prekės arba paslaugos pavadinimas")
                                    .Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Matav.\nvnt.").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Kiekis").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Kaina\n(EUR)").Bold();
                                header.Cell().Element(DefaultCellStyle).Text("Suma\n(EUR)").Bold();
                            });

                            for (int i = 0; i < invoiceDto.Products.Count; i++)
                            {
                                table.Cell().Element(CellStyle).Text((i + 1) + ".");
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].Name);
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].UnitOfMeasurement);
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].Units);
                                table.Cell().Element(CellStyle).Text(invoiceDto.Products[i].PriceOfUnit.ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT")));
                                table.Cell().Element(CellStyle)
                                    .Text((invoiceDto.Products[i].PriceOfUnit * invoiceDto.Products[i].Units).ToString("N2", CultureInfo.CreateSpecificCulture("lt-LT")));
                            }

                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Suma žodžiais: " + invoiceDto.SumInWords);
                            row.RelativeItem().AlignRight().Text("Suma iš viso:  " + invoiceDto.TotalMoney + " EUR").Bold();
                        });

                        column.Item().Column(column =>
                        {
                            column.Item().Text("Sąskaitą išrašė: " + invoiceDto.Name).FontSize(6);
                            column.Item().PaddingVertical(1).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                            column.Item()
                                .Text(
                                    "(asmens, atsakingo už ūkinės operacijos atlikimą ir teisingą įforminimą, pareigos, vardas, pavardė, parašas)")
                                .FontSize(6);
                        });

                        column.Item().Column(column =>
                        {
                            column.Item().Text("Sąskaitą gavo:").FontSize(6);
                            column.Item().PaddingVertical(1).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        });
                    });
            })

        );
    }
    
    
}